using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

/// <summary>
/// DTO ответа с данными счёта на оплату.
/// Возвращается внутри SubscriptionResponse.
/// </summary>
/// <param name="Id">ID счёта.</param>
/// <param name="Amount">Сумма к оплате.</param>
/// <param name="Status">Статус - Pending / Paid / Failed.</param>
/// <param name="Purpose">Назначение счёта.</param>
/// <param name="PlanId">План, применяемый после оплаты.</param>
/// <param name="BillingPeriod">Период оплаты.</param>
/// <param name="DueDate">Срок оплаты.</param>
/// <param name="CreatedWhen">Дата создания счёта.</param>
/// <param name="PaidWhen">Дата оплаты (null если не оплачен).</param>

public record InvoiceResponse(
    Guid Id,
    decimal Amount,
    InvoiceStatus Status,
    InvoicePurpose Purpose,
    Guid PlanId,
    BillingPeriod BillingPeriod,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedWhen,
    DateTimeOffset? PaidWhen);
