using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations;

/// <inheritdoc />
public partial class init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "AspNetRoles",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Name = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>("text", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

        migrationBuilder.CreateTable(
            "AspNetUsers",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                LastName = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                FirstName = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                Description = table.Column<string>("character varying(160)", maxLength: 160, nullable: true),
                RefreshTokenVersion = table.Column<int>("integer", nullable: false),
                LastUsernameChangedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>("boolean", nullable: false),
                LastConfirmationEmailSentAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                BannedBy = table.Column<Guid>("uuid", nullable: true),
                BannedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                BanReason = table.Column<int>("integer", nullable: true),
                IsBanned = table.Column<bool>("boolean", nullable: false),
                UserName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>("boolean", nullable: false),
                PasswordHash = table.Column<string>("text", nullable: true),
                SecurityStamp = table.Column<string>("text", nullable: true),
                ConcurrencyStamp = table.Column<string>("text", nullable: true),
                PhoneNumber = table.Column<string>("text", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>("boolean", nullable: false),
                TwoFactorEnabled = table.Column<bool>("boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>("boolean", nullable: false),
                AccessFailedCount = table.Column<int>("integer", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_AspNetUsers", x => x.Id); });

        migrationBuilder.CreateTable(
            "Conversations",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_Conversations", x => x.Id); });

        migrationBuilder.CreateTable(
            "HashTags",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Tag = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_HashTags", x => x.Id); });

        migrationBuilder.CreateTable(
            "AspNetRoleClaims",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<Guid>("uuid", nullable: false),
                ClaimType = table.Column<string>("text", nullable: true),
                ClaimValue = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    x => x.RoleId,
                    "AspNetRoles",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "AspNetUserClaims",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<Guid>("uuid", nullable: false),
                ClaimType = table.Column<string>("text", nullable: true),
                ClaimValue = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    "FK_AspNetUserClaims_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "AspNetUserLogins",
            table => new
            {
                LoginProvider = table.Column<string>("text", nullable: false),
                ProviderKey = table.Column<string>("text", nullable: false),
                ProviderDisplayName = table.Column<string>("text", nullable: true),
                UserId = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    "FK_AspNetUserLogins_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "AspNetUserRoles",
            table => new
            {
                UserId = table.Column<Guid>("uuid", nullable: false),
                RoleId = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    x => x.RoleId,
                    "AspNetRoles",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_AspNetUserRoles_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "AspNetUserTokens",
            table => new
            {
                UserId = table.Column<Guid>("uuid", nullable: false),
                LoginProvider = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("text", nullable: false),
                Value = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    "FK_AspNetUserTokens_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Reports",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                SenderId = table.Column<Guid>("uuid", nullable: false),
                Reason = table.Column<int>("integer", nullable: true),
                OtherReason = table.Column<string>("character varying(255)", maxLength: 255, nullable: true),
                ContentType = table.Column<int>("integer", nullable: false),
                ContentId = table.Column<Guid>("uuid", nullable: false),
                Status = table.Column<int>("integer", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reports", x => x.Id);
                table.ForeignKey(
                    "FK_Reports_AspNetUsers_SenderId",
                    x => x.SenderId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "UserFollows",
            table => new
            {
                FollowerId = table.Column<Guid>("uuid", nullable: false),
                FollowingId = table.Column<Guid>("uuid", nullable: false),
                FollowedAt = table.Column<DateTime>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserFollows", x => new { x.FollowerId, x.FollowingId });
                table.ForeignKey(
                    "FK_UserFollows_AspNetUsers_FollowerId",
                    x => x.FollowerId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    "FK_UserFollows_AspNetUsers_FollowingId",
                    x => x.FollowingId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Videos",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Description = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
                UserId = table.Column<Guid>("uuid", nullable: false),
                ProccessedInPercents = table.Column<int>("integer", nullable: false),
                Status = table.Column<int>("integer", nullable: false),
                BanReason = table.Column<int>("integer", nullable: false),
                CommentCount = table.Column<int>("integer", nullable: false),
                LikeCount = table.Column<int>("integer", nullable: false),
                FavoriteCount = table.Column<int>("integer", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true),
                DeletedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>("boolean", nullable: false),
                BannedBy = table.Column<Guid>("uuid", nullable: true),
                BannedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsBanned = table.Column<bool>("boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Videos", x => x.Id);
                table.ForeignKey(
                    "FK_Videos_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "ConversationParticipant",
            table => new
            {
                UserId = table.Column<Guid>("uuid", nullable: false),
                ConversationId = table.Column<Guid>("uuid", nullable: false),
                LastReadAt = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConversationParticipant", x => new { x.ConversationId, x.UserId });
                table.ForeignKey(
                    "FK_ConversationParticipant_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_ConversationParticipant_Conversations_ConversationId",
                    x => x.ConversationId,
                    "Conversations",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Messages",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                SenderId = table.Column<Guid>("uuid", nullable: false),
                IsDelivered = table.Column<bool>("boolean", nullable: false),
                Content = table.Column<string>("character varying(255)", maxLength: 255, nullable: false),
                IsRead = table.Column<bool>("boolean", nullable: false),
                ConversationId = table.Column<Guid>("uuid", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true),
                DeletedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>("boolean", nullable: false),
                BannedBy = table.Column<Guid>("uuid", nullable: true),
                BannedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsBanned = table.Column<bool>("boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Messages", x => x.Id);
                table.ForeignKey(
                    "FK_Messages_AspNetUsers_SenderId",
                    x => x.SenderId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    "FK_Messages_Conversations_ConversationId",
                    x => x.ConversationId,
                    "Conversations",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Comments",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Text = table.Column<string>("text", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                VideoId = table.Column<Guid>("uuid", nullable: false),
                ParentCommentId = table.Column<Guid>("uuid", nullable: true),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true),
                DeletedBy = table.Column<Guid>("uuid", nullable: true),
                DeletedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>("boolean", nullable: false),
                BannedBy = table.Column<Guid>("uuid", nullable: true),
                BannedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                IsBanned = table.Column<bool>("boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", x => x.Id);
                table.ForeignKey(
                    "FK_Comments_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_Comments_Comments_ParentCommentId",
                    x => x.ParentCommentId,
                    "Comments",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_Comments_Videos_VideoId",
                    x => x.VideoId,
                    "Videos",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Favorites",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                VideoId = table.Column<Guid>("uuid", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Favorites", x => x.Id);
                table.ForeignKey(
                    "FK_Favorites_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_Favorites_Videos_VideoId",
                    x => x.VideoId,
                    "Videos",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "VideoHashTags",
            table => new
            {
                VideoId = table.Column<Guid>("uuid", nullable: false),
                HashTagId = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VideoHashTags", x => new { x.VideoId, x.HashTagId });
                table.ForeignKey(
                    "FK_VideoHashTags_HashTags_HashTagId",
                    x => x.HashTagId,
                    "HashTags",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_VideoHashTags_Videos_VideoId",
                    x => x.VideoId,
                    "Videos",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "VideoLikes",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                VideoId = table.Column<Guid>("uuid", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>("uuid", nullable: true),
                UpdatedAt = table.Column<DateTime>("timestamp with time zone", nullable: true),
                UpdatedBy = table.Column<Guid>("uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VideoLikes", x => x.Id);
                table.ForeignKey(
                    "FK_VideoLikes_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_VideoLikes_Videos_VideoId",
                    x => x.VideoId,
                    "Videos",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "CommentLikes",
            table => new
            {
                CommentId = table.Column<Guid>("uuid", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                CreatedAt = table.Column<DateTime>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CommentLikes", x => new { x.UserId, x.CommentId });
                table.ForeignKey(
                    "FK_CommentLikes_AspNetUsers_UserId",
                    x => x.UserId,
                    "AspNetUsers",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_CommentLikes_Comments_CommentId",
                    x => x.CommentId,
                    "Comments",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_AspNetRoleClaims_RoleId",
            "AspNetRoleClaims",
            "RoleId");

        migrationBuilder.CreateIndex(
            "RoleNameIndex",
            "AspNetRoles",
            "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_AspNetUserClaims_UserId",
            "AspNetUserClaims",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_AspNetUserLogins_UserId",
            "AspNetUserLogins",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_AspNetUserRoles_RoleId",
            "AspNetUserRoles",
            "RoleId");

        migrationBuilder.CreateIndex(
            "EmailIndex",
            "AspNetUsers",
            "NormalizedEmail");

        migrationBuilder.CreateIndex(
            "IX_AspNetUsers_Email",
            "AspNetUsers",
            "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_AspNetUsers_UserName",
            "AspNetUsers",
            "UserName",
            unique: true);

        migrationBuilder.CreateIndex(
            "UserNameIndex",
            "AspNetUsers",
            "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_CommentLikes_CommentId",
            "CommentLikes",
            "CommentId");

        migrationBuilder.CreateIndex(
            "IX_CommentLikes_UserId_CommentId",
            "CommentLikes",
            new[] { "UserId", "CommentId" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Comments_ParentCommentId",
            "Comments",
            "ParentCommentId");

        migrationBuilder.CreateIndex(
            "IX_Comments_UserId",
            "Comments",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Comments_VideoId",
            "Comments",
            "VideoId");

        migrationBuilder.CreateIndex(
            "IX_ConversationParticipant_UserId",
            "ConversationParticipant",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Favorites_UserId_VideoId",
            "Favorites",
            new[] { "UserId", "VideoId" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Favorites_VideoId",
            "Favorites",
            "VideoId");

        migrationBuilder.CreateIndex(
            "IX_Messages_ConversationId",
            "Messages",
            "ConversationId");

        migrationBuilder.CreateIndex(
            "IX_Messages_SenderId",
            "Messages",
            "SenderId");

        migrationBuilder.CreateIndex(
            "IX_Reports_SenderId_ContentId",
            "Reports",
            new[] { "SenderId", "ContentId" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_UserFollows_FollowingId",
            "UserFollows",
            "FollowingId");

        migrationBuilder.CreateIndex(
            "IX_VideoHashTags_HashTagId",
            "VideoHashTags",
            "HashTagId");

        migrationBuilder.CreateIndex(
            "IX_VideoLikes_UserId_VideoId",
            "VideoLikes",
            new[] { "UserId", "VideoId" },
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_VideoLikes_VideoId",
            "VideoLikes",
            "VideoId");

        migrationBuilder.CreateIndex(
            "IX_Videos_UserId",
            "Videos",
            "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "AspNetRoleClaims");

        migrationBuilder.DropTable(
            "AspNetUserClaims");

        migrationBuilder.DropTable(
            "AspNetUserLogins");

        migrationBuilder.DropTable(
            "AspNetUserRoles");

        migrationBuilder.DropTable(
            "AspNetUserTokens");

        migrationBuilder.DropTable(
            "CommentLikes");

        migrationBuilder.DropTable(
            "ConversationParticipant");

        migrationBuilder.DropTable(
            "Favorites");

        migrationBuilder.DropTable(
            "Messages");

        migrationBuilder.DropTable(
            "Reports");

        migrationBuilder.DropTable(
            "UserFollows");

        migrationBuilder.DropTable(
            "VideoHashTags");

        migrationBuilder.DropTable(
            "VideoLikes");

        migrationBuilder.DropTable(
            "AspNetRoles");

        migrationBuilder.DropTable(
            "Comments");

        migrationBuilder.DropTable(
            "Conversations");

        migrationBuilder.DropTable(
            "HashTags");

        migrationBuilder.DropTable(
            "Videos");

        migrationBuilder.DropTable(
            "AspNetUsers");
    }
}