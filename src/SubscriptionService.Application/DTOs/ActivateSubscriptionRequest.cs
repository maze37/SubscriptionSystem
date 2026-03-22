namespace SubscriptionService.Application.DTOs;

/// <summary>HTTP request модель для активации подписки после оплаты счёта.</summary>
/// <param name="InvoiceId">ID счёта который был оплачен.</param>
public record ActivateSubscriptionRequest(Guid InvoiceId);