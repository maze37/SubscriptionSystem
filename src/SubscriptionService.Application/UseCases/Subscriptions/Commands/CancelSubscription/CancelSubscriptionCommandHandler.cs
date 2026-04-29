using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;

/// <summary>
/// Обработчик команды CancelSubscriptionCommand.
/// Находит подписку и устанавливает CancelAtPeriodEnd = true.
/// </summary>
public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand, CancelSubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTime;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    /// <inheritdoc/>
    public async Task<Result<CancelSubscriptionResponse, Error>> Handle(
        CancelSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(command.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result<CancelSubscriptionResponse, Error>.Failure(
                Error.NotFound("subscription.not_found", $"Подписка с ID '{command.SubscriptionId}' не найдена."));

        var cancelResult = subscription.Cancel(_dateTime.UtcNow);
        if (cancelResult.IsFailure)
            return Result<CancelSubscriptionResponse, Error>.Failure(cancelResult.Error!);

        _subscriptionRepository.Update(subscription);

        var saveResult = await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        if (saveResult.IsFailure)
            return Result<CancelSubscriptionResponse, Error>.Failure(saveResult.Error!);

        return Result<CancelSubscriptionResponse, Error>.Success(
            new CancelSubscriptionResponse(subscription.Id));
    }
}
