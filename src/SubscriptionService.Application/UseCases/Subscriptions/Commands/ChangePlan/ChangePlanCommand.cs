using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

public record ChangePlanCommand(
    Guid SubscriptionId,
    Guid NewPlanId,
    Guid InvoiceId,
    DateTimeOffset ChangedWhen) : ICommand;