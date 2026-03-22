using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

/// <summary>
/// DTO ответа с данными счёта на оплату.
/// Возвращается внутри SubscriptionResponse.
/// </summary>
/// <param name="Id">ID счёта.</param>
/// <param name="Amount">Сумма к оплате.</param>
/// <param name="Status">Статус - Pending / Paid / Failed.</param>
/// <param name="DueDate">Срок оплаты.</param>
/// <param name="CreatedWhen">Дата создания счёта.</param>
/// <param name="PaidWhen">Дата оплаты (null если не оплачен).</param>

public record InvoiceResponse(
    Guid Id,
    decimal Amount,
    InvoiceStatus Status,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedWhen,
    DateTimeOffset? PaidWhen);