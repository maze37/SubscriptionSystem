using Core.Database;
using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

/// <summary>
/// Обработчик команды CancelSubscriptionCommand.
/// Находит подписку и устанавливает CancelAtPeriodEnd = true.
/// </summary>
public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand, CancelSubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ITransactionManager transactionManager,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository;
        _transactionManager = transactionManager;
        _dateTime = dateTime;
    }

    /// <inheritdoc/>
    public async Task<Result<CancelSubscriptionResponse, Error>> HandleAsync(
        CancelSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Request.SubscriptionId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.Request.SubscriptionId));

        var subscriptionResult = await _subscriptionRepository
            .GetByIdAsync(command.Request.SubscriptionId, cancellationToken);
        if (subscriptionResult.IsFailure)
            return subscriptionResult.Error;

        var subscription = subscriptionResult.Value;

        var cancelResult = subscription.Cancel(_dateTime.UtcNow);
        if (cancelResult.IsFailure)
            return cancelResult.Error;

        _subscriptionRepository.Update(subscription);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        return new CancelSubscriptionResponse(subscription.Id);
    }
}
