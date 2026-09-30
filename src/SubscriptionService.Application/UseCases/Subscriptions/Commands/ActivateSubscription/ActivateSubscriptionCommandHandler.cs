using Core.Database;
using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Обработчик команды ActivateSubscriptionCommand.
/// Находит подписку, отмечает счёт как оплаченный, активирует подписку.
/// </summary>
public class ActivateSubscriptionCommandHandler : ICommandHandler<ActivateSubscriptionCommand, ActivateSubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public ActivateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        ITransactionManager transactionManager,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository;
        _transactionManager = transactionManager;
        _dateTime = dateTime;
    }

    /// <inheritdoc/>
    public async Task<Result<ActivateSubscriptionResponse, Error>> HandleAsync(
        ActivateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.SubscriptionId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.SubscriptionId));

        if (command.Request.InvoiceId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.Request.InvoiceId));

        var subscriptionResult = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscriptionResult.IsFailure)
            return subscriptionResult.Error;

        var subscription = subscriptionResult.Value;

        var activateResult = subscription.Activate(
            command.Request.InvoiceId,
            _dateTime.UtcNow);
        if (activateResult.IsFailure)
            return activateResult.Error;

        _subscriptionRepository.Update(subscription);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        return new ActivateSubscriptionResponse(subscription.Id, command.Request.InvoiceId);
    }
}
