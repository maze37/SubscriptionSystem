using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

/// <summary>
/// Обработчик команды ChangePlanCommand.
/// Проверяет план, меняет PlanId и создаёт новый счёт на оплату.
/// </summary>
public class ChangePlanCommandHandler : ICommandHandler<ChangePlanCommand, ChangePlanResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTime;

    public ChangePlanCommandHandler(
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
    public async Task<Result<ChangePlanResponse, Error>> Handle(
        ChangePlanCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result<ChangePlanResponse, Error>.Failure(
                Error.NotFound("subscription.not_found", $"Подписка с ID '{command.SubscriptionId}' не найдена."));

        var plan = await _planRepository
            .GetByIdAsync(command.NewPlanId, cancellationToken)
            .ConfigureAwait(false);

        if (plan is null)
            return Result<ChangePlanResponse, Error>.Failure(
                Error.NotFound("plan.not_found", $"План с ID '{command.NewPlanId}' не найден."));

        if (!plan.IsActive)
            return Result<ChangePlanResponse, Error>.Failure(
                Error.Conflict("plan.inactive", "Нельзя сменить на неактивный план."));

        var changeResult = subscription.ChangePlan(
            Guid.NewGuid(),
            command.NewPlanId,
            plan.Price,
            _dateTime.UtcNow);
        if (changeResult.IsFailure)
            return Result<ChangePlanResponse, Error>.Failure(changeResult.Error!);

        _subscriptionRepository.Update(subscription);

        var saveResult = await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        if (saveResult.IsFailure)
            return Result<ChangePlanResponse, Error>.Failure(saveResult.Error!);

        return Result<ChangePlanResponse, Error>.Success(
            new ChangePlanResponse(subscription.Id, command.NewPlanId));
    }
}
