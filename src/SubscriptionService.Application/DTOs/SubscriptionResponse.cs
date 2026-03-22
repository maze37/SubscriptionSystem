using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

/// <summary>
/// DTO ответа с данными подписки.
/// Используется в GetSubscription.
/// </summary>
/// <param name="Id">ID подписки.</param>
/// <param name="UserId">ID пользователя.</param>
/// <param name="PlanId">ID тарифного плана.</param>
/// <param name="Status">Текущий статус подписки.</param>
/// <param name="CurrentPeriodEnd">Дата окончания текущего периода.</param>
/// <param name="CancelAtPeriodEnd">Будет ли подписка отменена в конце периода.</param>
/// <param name="TrialEnd">Дата окончания триала (null если без триала).</param>
/// <param name="CreatedWhen">Дата создания подписки.</param>
/// <param name="Invoices">История счетов на оплату.</param>
public record SubscriptionResponse(
    Guid Id,
    Guid UserId,
    Guid PlanId,
    SubscriptionStatus Status,
    DateTimeOffset CurrentPeriodEnd,
    bool CancelAtPeriodEnd,
    DateTimeOffset? TrialEnd,
    DateTimeOffset CreatedWhen,
    IReadOnlyList<InvoiceResponse> Invoices);