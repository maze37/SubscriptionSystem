using SubscriptionService.Application.Abstractions.Core;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

public record CancelSubscriptionCommand(
    Guid SubscriptionId,
    DateTimeOffset CancelledWhen) : ICommand;