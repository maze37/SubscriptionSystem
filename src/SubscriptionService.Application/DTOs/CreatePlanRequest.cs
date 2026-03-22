using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

public record CreatePlanRequest(string Name, decimal Price, BillingPeriod BillingPeriod);