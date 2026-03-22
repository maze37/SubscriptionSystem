using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

public record ActivateSubscriptionCommand(
    Guid SubscriptionId,
    Guid InvoiceId,
    DateTimeOffset ActivatedWhen) : ICommand;