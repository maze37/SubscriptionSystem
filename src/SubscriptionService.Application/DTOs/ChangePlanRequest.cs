namespace SubscriptionService.Application.DTOs;

/// <summary>HTTP request модель для смены тарифного плана.</summary>
/// <param name="NewPlanId">ID нового тарифного плана.</param>
public record ChangePlanRequest(Guid NewPlanId);