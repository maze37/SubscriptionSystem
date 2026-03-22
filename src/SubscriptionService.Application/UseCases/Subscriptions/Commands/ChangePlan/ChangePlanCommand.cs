using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

/// <summary>
/// Команда смены тарифного плана.
/// Создаётся новый счёт на оплату по цене нового плана.
/// </summary>
/// <param name="SubscriptionId">ID подписки.</param>
/// <param name="NewPlanId">ID нового плана.</param>
public record ChangePlanCommand(
    Guid SubscriptionId,
    Guid NewPlanId) : ICommand;