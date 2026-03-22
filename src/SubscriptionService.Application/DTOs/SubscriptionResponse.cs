using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

public record SubscriptionResponse(
    Guid Id,
    Guid UserId,
    Guid PlanId,
    SubscriptionStatus Status,
    DateTimeOffset CurrentPeriodEnd,
    bool CancelAtPeriodEnd,
    DateTimeOffset? TrialEnd,
    DateTimeOffset CreatedWhen,
    IReadOnlyList<InvoiceResponse> Invoices);