using SharedKernel.Result;

namespace SubscriptionService.Web.Contracts;

internal static class EndpointResult
{
    public static EndpointEnvelope<T> Success<T>(T data, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<T>(true, data, null, context.TraceIdentifier);
    }

    public static EndpointEnvelope<object?> Success(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(true, null, null, context.TraceIdentifier);
    }

    public static EndpointEnvelope<object?> Failure(Error error, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentNullException.ThrowIfNull(context);
        return new EndpointEnvelope<object?>(false, null, error, context.TraceIdentifier);
    }
}
