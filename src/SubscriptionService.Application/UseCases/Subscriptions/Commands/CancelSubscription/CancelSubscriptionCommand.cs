using Core.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

/// <summary>
/// Команда отмены подписки.
/// Доступ сохраняется до конца текущего периода (CancelAtPeriodEnd = true).
/// </summary>
public record CancelSubscriptionCommand(CancelSubscriptionRequest Request) : ICommand;
