using Application;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment webHostEnvironment;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        webHostEnvironment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogInformation(ex, "Помилка валідації");
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.ValidationError(ex.Errors));
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex, "Не авторизований");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Error(ex.Message));
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation(ex, "Не знайдено");
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Error(ex.Message));
        }
        catch (NotAllowedException ex)
        {
            _logger.LogWarning(ex, "Недостатньо прав");
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(ApiResponse<object?>.ErrorWithPayload(ex.Payload, ex.Message));
        }
        catch (BadRequestException ex)
        {
            _logger.LogInformation(ex, "Поганий запит");
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Error(null,$"Внутрішня помилка сервера: {ex.Message}"));
                return;
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Error(ErrorCodes.InternalServerError,"Внутрішня помилка сервера"));
        }
    }
}