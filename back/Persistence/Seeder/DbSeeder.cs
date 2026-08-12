using System.Text.Json;
using Application.Constants;
using Application.Dtos.User;
using Application.Interfaces;
using Application.Options;
using Domain;
using Domain.Entities.Identity;
using Domain.Entities.Video;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NanoidDotNet;
using Serilog;

namespace Persistence.Seeder;

public static class DbSeeder
{
    // All json seed files should be located in Tiktok_Clone/Helpers and have Copy To Output Directory
    public static async Task SeedDataAsync(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var localStorageOptions = scope.ServiceProvider.GetRequiredService<IOptions<LocalStorageOptions>>();
        
        var _options = localStorageOptions.Value;
        await context.Database.MigrateAsync();
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager, imageService, environment);

        if (environment.IsDevelopment())
        {
            await SeedVideosAsync(context, environment, _options, userManager);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<RoleEntity> roleManager)
    {
        if (!roleManager.Roles.Any())
        {
            var roles = new List<string>
            {
                RoleNames.USER_ROLE,
                RoleNames.ADMIN_ROLE,
                RoleNames.SUPER_ADMIN_ROLE
            };
            foreach (var role in roles)
            {
                var newRole = new RoleEntity
                {
                    Name = role
                };

                var result = await roleManager.CreateAsync(newRole);

                if (result.Succeeded)
                    Log.Information("Role {RoleName} seeded successfully", role);
                else
                    Log.Error("Failed to seed role {RoleName}. Errors : {Errors}",
                        role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Log.Information("Roles already exists in database, skipping seeding");
        }
    }

    private static async Task SeedUsersAsync(UserManager<UserEntity> userManager, IImageService imageService,
        IWebHostEnvironment environment)
    {
        if (!userManager.Users.Any())
        {
            var json = await File.ReadAllTextAsync(Path.Combine(environment.ContentRootPath, "Helpers",
                "Users.json"));
            var users = JsonSerializer.Deserialize<List<SeedUserDto>>(json);

            if (users == null)
            {
                Log.Error("Failed to get users from json file to seed databse");
                return;
            }

            foreach (var user in users)
            {
                var newUser = new UserEntity
                {
                    UserName = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                var result = await userManager.CreateAsync(newUser, user.Password!);
                if (result.Succeeded)
                {
                    var resultR = await userManager.AddToRolesAsync(newUser, user.Roles!);
                    if (resultR.Succeeded)
                    {
                        newUser.EmailConfirmed = true;
                        await imageService.SaveImageAsync(user.Image!, newUser.Id);
                        await userManager.UpdateAsync(newUser);
                        Log.Information("User {UserName} seeded successfully", user.Username);
                    }
                    else
                    {
                        Log.Error("Failed to assign roles. Error: {Errors}",
                            string.Join(", ", resultR.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    Log.Error("Failed to seed user {UserName}. Errors : {Errors}",
                        user.Username,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        else
        {
            Log.Information("Users already exists in database, skipping seeding");
        }
    }

    private static async Task SeedVideosAsync(AppDbContext context, IWebHostEnvironment environment, LocalStorageOptions localStorageOptions, UserManager<UserEntity> userManager)
    {
        var seedVideoFolder = Path.Combine(environment.ContentRootPath, "SeedVideos");
        var outputFolder = Path.Combine(localStorageOptions.RootPath, "uploads", "processed");
        Directory.CreateDirectory(seedVideoFolder);
        var userIds = userManager.Users.Select(u => u.Id).ToArray();
        
        foreach (var folder in Directory.GetDirectories(seedVideoFolder))
        {
            var guid = Guid.NewGuid();
            var videoFolder = Directory.CreateDirectory(Path.Combine(outputFolder, guid.ToString()));

            CopyDirectory(folder, videoFolder.FullName);
            
            context.Videos
                .Add(new VideoEntity
                {
                    Id = guid,
                    ShortId = await Nanoid.GenerateAsync(
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 9),
                    UserId = userIds[Random.Shared.Next(0, userIds.Length)],
                    Status = VideoStatus.Processed,
                    ProccessedInPercents = 100
                });

            await context.SaveChangesAsync();
        }
    }

    private static void CopyDirectory(string source, string dest, bool recursive = true)
    {
        var dir = new DirectoryInfo(source);
        if(!dir.Exists) throw new DirectoryNotFoundException();

        Directory.CreateDirectory(dest);

        foreach (FileInfo file in dir.GetFiles())
        {
            file.CopyTo(Path.Combine(dest, file.Name), true);
        }

        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            CopyDirectory(subDir.FullName, Path.Combine(dest, subDir.Name));
        }
    }
}