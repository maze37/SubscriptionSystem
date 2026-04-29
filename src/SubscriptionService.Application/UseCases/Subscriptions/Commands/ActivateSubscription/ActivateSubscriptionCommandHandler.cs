using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

/// <summary>
/// Обработчик команды ActivateSubscriptionCommand.
/// Находит подписку, отмечает счёт как оплаченный, активирует подписку.
/// </summary>
public class ActivateSubscriptionCommandHandler : ICommandHandler<ActivateSubscriptionCommand, ActivateSubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTime;

    public ActivateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    /// <inheritdoc/>
    public async Task<Result<ActivateSubscriptionResponse, Error>> Handle(
        ActivateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result<ActivateSubscriptionResponse, Error>.Failure(
                Error.NotFound("subscription.not_found", $"Подписка с ID '{command.SubscriptionId}' не найдена."));

        var plan = await _planRepository
            .GetByIdAsync(subscription.PlanId, cancellationToken)
            .ConfigureAwait(false);

        if (plan is null)
            return Result<ActivateSubscriptionResponse, Error>.Failure(
                Error.NotFound("plan.not_found", $"План с ID '{subscription.PlanId}' не найден."));

        var activateResult = subscription.Activate(command.InvoiceId, plan.BillingPeriod, _dateTime.UtcNow);
        if (activateResult.IsFailure)
            return Result<ActivateSubscriptionResponse, Error>.Failure(activateResult.Error!);

        _subscriptionRepository.Update(subscription);

        var saveResult = await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        if (saveResult.IsFailure)
            return Result<ActivateSubscriptionResponse, Error>.Failure(saveResult.Error!);

        return Result<ActivateSubscriptionResponse, Error>.Success(
            new ActivateSubscriptionResponse(subscription.Id, command.InvoiceId));
    }
}
