using SharedKernel.Result;

namespace SubscriptionService.Web.Contracts;

internal static class EndpointResult
{
    public static EndpointEnvelope<T> Success<T>(T data, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<T>(data, null, DateTimeOffset.UtcNow);
    }

    public static EndpointEnvelope<object?> Success(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(null, null, DateTimeOffset.UtcNow);
    }

    public static EndpointEnvelope<object?> Failure(Error error, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(null, [error], DateTimeOffset.UtcNow);
    }

    public static EndpointEnvelope<object?> Failure(ErrorList errors, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(null, errors.AsReadOnly(), DateTimeOffset.UtcNow);
    }

}
