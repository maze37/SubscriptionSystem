namespace SubscriptionService.Application.DTOs;

public record GetActivePlansResponse(IReadOnlyList<PlanResponse> Plans);
