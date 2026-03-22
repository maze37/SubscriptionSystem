namespace SubscriptionService.Domain.Enums;

/// <summary>Статус подписки.</summary>
public enum SubscriptionStatus
{
    Trial = 1,
    Active = 2,
    Cancelled = 3,
    Expired = 4,
    PastDue = 5
}