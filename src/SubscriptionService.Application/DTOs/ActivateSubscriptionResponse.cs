namespace SubscriptionService.Application.DTOs;

public record ActivateSubscriptionResponse(Guid SubscriptionId, Guid InvoiceId);
