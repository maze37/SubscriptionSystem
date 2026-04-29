using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Команда активации подписки после оплаты счёта.
/// InvoiceId приходит с клиента — нужен конкретный счёт который оплачен.
/// </summary>
/// <param name="SubscriptionId">ID подписки.</param>
/// <param name="InvoiceId">ID оплаченного счёта.</param>
public record ActivateSubscriptionCommand(
    Guid SubscriptionId,
    Guid InvoiceId) : ICommand<ActivateSubscriptionResponse>;

public record ActivateSubscriptionResponse(Guid SubscriptionId, Guid InvoiceId);
