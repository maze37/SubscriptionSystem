using SharedKernel.Result;

namespace SubscriptionService.Web.Contracts;

public sealed record EndpointEnvelope<T>(
    bool Success,
    T? Data,
    Error? Error,
    string TraceId);
