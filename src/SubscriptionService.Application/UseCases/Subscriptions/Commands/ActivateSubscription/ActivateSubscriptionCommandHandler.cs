using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;

public class ActivateSubscriptionCommandHandler : ICommandHandler<ActivateSubscriptionCommand>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(
        ActivateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result.Failure(
                Error.NotFound($"Подписка с ID '{command.SubscriptionId}' не найдена."));

        subscription.Activate(command.InvoiceId, command.ActivatedWhen);

        _subscriptionRepository.Update(subscription);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result.Success();
    }
}