namespace SubscriptionService.Application.DTOs;

public record CreateSubscriptionRequest(Guid UserId, Guid PlanId, bool WithTrial);