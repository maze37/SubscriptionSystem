namespace SubscriptionService.Domain.Enums;

/// <summary>
/// Статус подписки.
/// </summary>
public enum SubscriptionStatus
{
    PendingPayment = 1,
    Trial = 2,
    Active = 3,
    PastDue = 4,
    Cancelled = 5,
    Expired = 6
}