using FluentValidation;
using Microsoft.Extensions.Localization;
using Review_Guard.API.Resources;
using Review_Guard.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Review_Guard.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var localizer = context.RequestServices
                .GetRequiredService<IStringLocalizer<SharedResource>>();

            await HandleExceptionAsync(context, ex, localizer);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IStringLocalizer<SharedResource> localizer)
    {
        if (context.Response.HasStarted)
            return;

        var traceId = context.TraceIdentifier;

        _logger.LogError(
            exception,
            "Unhandled Exception | TraceId: {TraceId} | Path: {Path}",
            traceId,
            context.Request.Path);

        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode;
        string errorCode;
        string message;
        Dictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case JsonException jsonEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "Server.InvalidRequestFormat";
                message = _env.IsDevelopment()
                    ? jsonEx.Message
                    : "Invalid JSON format.";
                break;

            case ValidationException validationEx:
                {
                    statusCode = HttpStatusCode.BadRequest;
                    errorCode = "Validation.Failed";
                    message = localizer["Validation.ValidationFailed"].Value;

                    errors = validationEx.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x =>
                                localizer[x.ErrorMessage].Value // 👈 هنا الترجمة
                            ).ToArray()
                        );
                    break;
                }

            case DomainException domainEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = domainEx.MessageKey;
                message = localizer[domainEx.MessageKey].Value;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorCode = "Auth.Unauthorized";
                message = localizer["Common.Unauthorized"].Value;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                errorCode = "Server.InternalError";
                message = _env.IsDevelopment()
                    ? exception.Message
                    : localizer["Common.UnexpectedError"].Value;
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            success = false,
            errorCode,
            message,
            errors,
            traceId,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}