using System.Net;
using System.Text.Json;
using SharedKernel.Exceptions;
using SharedKernel.Result;
using SubscriptionService.Web.Contracts;

namespace SubscriptionService.Web.Middlewares;

/// <summary>
/// Middleware для централизованной обработки исключений.
/// Перехватывает все необработанные исключения и возвращает
/// структурированный JSON ответ вместо HTML стектрейса.
/// </summary>
public sealed class ExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (DomainException ex)
        {
            // Доменные ошибки — ожидаемые бизнес-нарушения, логируем как Warning
            _logger.LogWarning(ex, "Domain exception: {Code} - {Message}", ex.Code, ex.Message);
            await HandleDomainExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Всё остальное — непредвиденная ошибка, логируем как Error
            _logger.LogError(ex, "Unhandled exception");
            await HandleUnhandledExceptionAsync(context).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Возвращает 400 Bad Request с деталями доменной ошибки.
    /// </summary>
    private static async Task HandleDomainExceptionAsync(HttpContext context, DomainException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var error = Error.Validation(ex.Message, ex.Field);
        var response = EndpointResult.Failure(error, context);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает 500 Internal Server Error без деталей — стектрейс клиенту не отдаём.
    /// </summary>
    private static async Task HandleUnhandledExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var response = EndpointResult.Failure(
            Error.Failure("Произошла непредвиденная ошибка."),
            context);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions))
            .ConfigureAwait(false);
    }
}
