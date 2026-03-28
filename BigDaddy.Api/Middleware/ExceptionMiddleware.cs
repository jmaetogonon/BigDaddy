using BigDaddy.Application.Common;
using BigDaddy.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BigDaddy.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception ex)
    {
        var (code, message) = ex switch
        {
            NotFoundException => (HttpStatusCode.NotFound, ex.Message),
            ConflictException => (HttpStatusCode.Conflict, ex.Message),
            ForbiddenException => (HttpStatusCode.Forbidden, ex.Message),
            DomainException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(
            ApiResponse.Fail(message),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return context.Response.WriteAsync(body);
    }
}
