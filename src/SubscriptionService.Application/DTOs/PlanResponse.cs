using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

public record PlanResponse(
    Guid Id,
    string Name,
    decimal Price,
    BillingPeriod BillingPeriod,
    bool IsActive);