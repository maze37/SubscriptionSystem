using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(
        CancelSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result.Failure(
                Error.NotFound($"Подписка с ID '{command.SubscriptionId}' не найдена."));

        subscription.Cancel(command.CancelledWhen);

        _subscriptionRepository.Update(subscription);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result.Success();
    }
}