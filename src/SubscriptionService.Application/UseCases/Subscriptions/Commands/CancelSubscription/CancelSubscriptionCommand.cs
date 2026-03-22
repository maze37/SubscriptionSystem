using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

/// <summary>
/// Команда отмены подписки.
/// Доступ сохраняется до конца текущего периода (CancelAtPeriodEnd = true).
/// </summary>
/// <param name="SubscriptionId">ID подписки.</param>
public record CancelSubscriptionCommand(Guid SubscriptionId) : ICommand;