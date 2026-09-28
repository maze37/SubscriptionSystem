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
    private readonly IPlanRepository _planRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public ActivateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        ITransactionManager transactionManager,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
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

        var transactionResult = await _transactionManager
            .BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
            return transactionResult.Error;

        using var transaction = transactionResult.Value;

        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken);

        if (subscription.IsFailure)
            return subscription.Error;

        var plan = await _planRepository
            .GetByIdAsync(subscription.Value.PlanId, cancellationToken);

        if (plan.IsFailure)
            return plan.Error;

        var activateResult = subscription.Value.Activate(
            command.Request.InvoiceId,
            plan.Value.BillingPeriod,
            _dateTime.UtcNow);
        if (activateResult.IsFailure)
            return activateResult.Error;

        _subscriptionRepository.Update(subscription.Value);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
            return commitResult.Error;

        return new ActivateSubscriptionResponse(subscription.Value.Id, command.Request.InvoiceId);
    }
}
