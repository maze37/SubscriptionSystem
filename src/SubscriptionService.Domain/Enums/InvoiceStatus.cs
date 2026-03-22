namespace SubscriptionService.Domain.Enums;

/// <summary>Статус счёта на оплату.</summary>
public enum InvoiceStatus
{
    Pending = 1,
    Paid = 2,
    Failed = 3
}