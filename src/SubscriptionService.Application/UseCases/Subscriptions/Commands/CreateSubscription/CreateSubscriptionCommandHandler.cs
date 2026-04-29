using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Domain.Aggregates.Subscription;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Обработчик команды CreateSubscriptionCommand.
/// Проверяет пользователя, триал, активные подписки и план.
/// Создаёт подписку и сохраняет.
/// </summary>
public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTime;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IPlanRepository planRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    /// <inheritdoc/>
    public async Task<Result<Guid, Error>> Handle(
        CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByIdAsync(command.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return Result<Guid, Error>.Failure(
                Error.NotFound("user.not_found", $"Пользователь с ID '{command.UserId}' не найден."));

        if (command.WithTrial && user.HasUsedTrial)
            return Result<Guid, Error>.Failure(
                Error.Conflict("user.trial_already_used", "Триальный период уже был использован."));

        var hasActive = await _subscriptionRepository
            .HasActiveSubscriptionAsync(command.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (hasActive)
            return Result<Guid, Error>.Failure(
                Error.Conflict("subscription.active_exists", "У пользователя уже есть активная подписка."));

        var plan = await _planRepository
            .GetByIdAsync(command.PlanId, cancellationToken)
            .ConfigureAwait(false);

        if (plan is null)
            return Result<Guid, Error>.Failure(
                Error.NotFound("plan.not_found", $"План с ID '{command.PlanId}' не найден."));

        if (!plan.IsActive)
            return Result<Guid, Error>.Failure(
                Error.Conflict("plan.inactive", "Нельзя подписаться на неактивный план."));

        if (command.WithTrial)
        {
            var markTrialResult = user.MarkTrialUsed();
            if (markTrialResult.IsFailure)
                return Result<Guid, Error>.Failure(markTrialResult.Error!);
        }

        var subscription = Subscription.Create(
            Guid.NewGuid(),
            command.UserId,
            command.PlanId,
            Guid.NewGuid(),
            plan.Price,
            plan.BillingPeriod,
            command.WithTrial,
            _dateTime.UtcNow);
        if (subscription.IsFailure)
            return Result<Guid, Error>.Failure(subscription.Error!);

        _subscriptionRepository.Add(subscription.Value!);

        var saveResult = await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        if (saveResult.IsFailure)
            return Result<Guid, Error>.Failure(saveResult.Error!);

        return Result<Guid, Error>.Success(subscription.Value!.Id);
    }
}
