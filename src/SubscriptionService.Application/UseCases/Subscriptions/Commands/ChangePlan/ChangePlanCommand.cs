using Core.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

/// <summary>
/// Команда смены тарифного плана.
/// Создаётся новый счёт на оплату по цене нового плана.
/// </summary>
public record ChangePlanCommand(Guid SubscriptionId, ChangePlanRequest Request) : ICommand;
