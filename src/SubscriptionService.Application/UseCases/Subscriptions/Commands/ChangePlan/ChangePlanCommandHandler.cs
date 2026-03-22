using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;

public class ChangePlanCommandHandler : ICommandHandler<ChangePlanCommand>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePlanCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(
        ChangePlanCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result.Failure(
                Error.NotFound($"Подписка с ID '{command.SubscriptionId}' не найдена."));

        var plan = await _planRepository
            .GetByIdAsync(command.NewPlanId, cancellationToken)
            .ConfigureAwait(false);

        if (plan is null)
            return Result.Failure(
                Error.NotFound($"План с ID '{command.NewPlanId}' не найден."));

        if (!plan.IsActive)
            return Result.Failure(
                Error.Conflict("Нельзя сменить на неактивный план."));

        subscription.ChangePlan(
            command.InvoiceId,
            command.NewPlanId,
            plan.Price,
            command.ChangedWhen);

        _subscriptionRepository.Update(subscription);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result.Success();
    }
}