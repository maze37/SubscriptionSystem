namespace SubscriptionService.Application.DTOs;

public record ChangePlanResponse(Guid SubscriptionId, Guid NewPlanId);
