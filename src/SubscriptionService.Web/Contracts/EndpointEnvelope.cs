using SharedKernel.Result;

namespace SubscriptionService.Web.Contracts;

public sealed record EndpointEnvelope<T>(
    T? Result,
    IReadOnlyList<Error>? Errors,
    DateTimeOffset TimeGenerated);
