using SharedKernel.Result;

namespace SubscriptionService.Web.Contracts;

/// <summary>
/// Фабрика envelope-ответов для успешных и ошибочных сценариев API.
/// </summary>
internal static class EndpointResult
{
    /// <summary>Сформировать успешный ответ с данными.</summary>
    public static EndpointEnvelope<T> Success<T>(T data, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<T>(data, null, DateTimeOffset.UtcNow);
    }

    /// <summary>Сформировать успешный ответ без данных.</summary>
    public static EndpointEnvelope<object?> Success(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(null, null, DateTimeOffset.UtcNow);
    }

    /// <summary>Сформировать ошибочный ответ с одной ошибкой.</summary>
    public static EndpointEnvelope<object?> Failure(Error error, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(null, [error], DateTimeOffset.UtcNow);
    }

    /// <summary>Сформировать ошибочный ответ со списком ошибок.</summary>
    public static EndpointEnvelope<object?> Failure(ErrorList errors, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(null, errors.AsReadOnly(), DateTimeOffset.UtcNow);
    }

}
