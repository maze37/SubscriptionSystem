using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Команда создания новой подписки.
/// Id и время генерируются в хендлере.
/// </summary>
/// <param name="UserId">ID пользователя.</param>
/// <param name="PlanId">ID тарифного плана.</param>
/// <param name="WithTrial">Использовать триальный период.</param>
public record CreateSubscriptionCommand(
    Guid UserId,
    Guid PlanId,
    bool WithTrial) : ICommand<Guid>;