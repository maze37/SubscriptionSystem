using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

/// <summary>Команда создания нового тарифного плана.</summary>
/// <param name="Name">Название плана.</param>
/// <param name="Price">Цена плана - больше нуля.</param>
/// <param name="BillingPeriod">Период оплаты — месяц или год.</param>
public record CreatePlanCommand(
    string Name,
    decimal Price,
    BillingPeriod BillingPeriod) : ICommand<Guid>;