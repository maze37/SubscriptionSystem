using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

public record CreatePlanCommand(
    Guid Id,
    string Name,
    decimal Price,
    BillingPeriod BillingPeriod,
    DateTimeOffset CreatedWhen) : ICommand<Guid>;