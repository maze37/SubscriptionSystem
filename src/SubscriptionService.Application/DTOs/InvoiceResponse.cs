using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

public record InvoiceResponse(
    Guid Id,
    decimal Amount,
    InvoiceStatus Status,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedWhen,
    DateTimeOffset? PaidWhen);