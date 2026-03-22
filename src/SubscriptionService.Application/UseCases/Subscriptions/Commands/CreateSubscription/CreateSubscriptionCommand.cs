using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;

public record CreateSubscriptionCommand(
    Guid Id,
    Guid UserId,
    Guid PlanId,
    Guid InvoiceId,
    bool WithTrial,
    DateTimeOffset CreatedWhen) : ICommand<Guid>;