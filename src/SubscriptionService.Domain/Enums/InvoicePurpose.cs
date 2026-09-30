namespace SubscriptionService.Domain.Enums;

/// <summary>
/// Назначение счёта на оплату.
/// Определяет изменение подписки после оплаты.
/// </summary>
public enum InvoicePurpose
{
    InitialSubscription = 1,
    ChangePlan = 2,
    Renewal = 3
}
