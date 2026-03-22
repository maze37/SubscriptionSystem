namespace SubscriptionService.Web.Middlewares;

/// <summary>
/// Middleware для логирования HTTP запросов.
/// Логирует метод, путь, статус код и время выполнения.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTimeOffset.UtcNow;

        await _next(context).ConfigureAwait(false);

        var elapsed = DateTimeOffset.UtcNow - start;

        _logger.LogInformation(
            "HTTP {Method} {Path} → {StatusCode} за {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsed.TotalMilliseconds.ToString("F0"));
    }
}