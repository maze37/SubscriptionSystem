using Core.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Команда создания новой подписки.
/// </summary>
public record CreateSubscriptionCommand(CreateSubscriptionRequest Request) : ICommand;
