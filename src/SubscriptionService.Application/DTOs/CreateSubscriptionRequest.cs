namespace SubscriptionService.Application.DTOs;

/// <summary>HTTP request модель для создания подписки.</summary>
/// <param name="UserId">ID пользователя.</param>
/// <param name="PlanId">ID тарифного плана.</param>
/// <param name="WithTrial">Использовать триальный период (14 дней бесплатно).</param>
public record CreateSubscriptionRequest(Guid UserId, Guid PlanId, bool WithTrial);