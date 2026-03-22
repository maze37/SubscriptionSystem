using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

/// <summary>
/// DTO ответа с данными тарифного плана.
/// Используется в GetActivePlans.
/// </summary>
/// <param name="Id">ID плана.</param>
/// <param name="Name">Название плана.</param>
/// <param name="Price">Цена плана.</param>
/// <param name="BillingPeriod">Период оплаты - Monthly или Yearly.</param>
/// <param name="IsActive">Активен ли план - можно ли на него подписаться.</param>
public record PlanResponse(
    Guid Id,
    string Name,
    decimal Price,
    BillingPeriod BillingPeriod,
    bool IsActive);