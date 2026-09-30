using Core.Database;
using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

/// <summary>
/// Обработчик команды ChangePlanCommand.
/// Проверяет план, меняет PlanId и создаёт новый счёт на оплату.
/// </summary>
public class ChangePlanCommandHandler : ICommandHandler<ChangePlanCommand, ChangePlanResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public ChangePlanCommandHandler(
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
    public async Task<Result<ChangePlanResponse, Error>> HandleAsync(
        ChangePlanCommand command,
        CancellationToken cancellationToken)
    {
        if (command.SubscriptionId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.SubscriptionId));

        if (command.Request.NewPlanId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.Request.NewPlanId));

        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription.IsFailure)
            return subscription.Error;

        var plan = await _planRepository
            .GetByIdAsync(command.Request.NewPlanId, cancellationToken);
        if (plan.IsFailure)
            return plan.Error;

        if (!plan.Value.IsActive)
            return GeneralErrors.InvalidOperation("Нельзя сменить на неактивный план.");

        var changeResult = subscription.Value.ChangePlan(
            Guid.NewGuid(),
            plan.Value.Id,
            plan.Value.Price,
            plan.Value.BillingPeriod,
            _dateTime.UtcNow);
        if (changeResult.IsFailure)
            return changeResult.Error;

        _subscriptionRepository.Update(subscription.Value);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        return new ChangePlanResponse(subscription.Value.Id, command.Request.NewPlanId);
    }
}
