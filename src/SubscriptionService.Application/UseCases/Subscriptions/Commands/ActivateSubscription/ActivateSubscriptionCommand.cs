using Core.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Команда активации подписки после оплаты счёта.
/// InvoiceId приходит с клиента - нужен конкретный счёт который оплачен.
/// </summary>
public record ActivateSubscriptionCommand(
    Guid SubscriptionId,
    ActivateSubscriptionRequest Request) : ICommand;
