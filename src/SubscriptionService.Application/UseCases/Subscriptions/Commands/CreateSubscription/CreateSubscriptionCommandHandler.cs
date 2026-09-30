using Core.Database;
using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Aggregates.Subscription;

namespace SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Обработчик команды CreateSubscriptionCommand.
/// Проверяет пользователя, триал, активные подписки и план.
/// Создаёт подписку и сохраняет.
/// </summary>
public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IPlanRepository planRepository,
        ITransactionManager transactionManager,
        IDateTimeProvider dateTime)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _planRepository = planRepository;
        _transactionManager = transactionManager;
        _dateTime = dateTime;
    }

    /// <inheritdoc/>
    public async Task<Result<CreateSubscriptionResponse, Error>> HandleAsync(
        CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Request.PlanId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.Request.PlanId), "ID плана не может быть пустым.");

        if (command.Request.UserId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(command.Request.UserId), "ID пользователя не может быть пустым.");

        var user = await _userRepository
            .GetByIdAsync(command.Request.UserId, cancellationToken);
        if (user.IsFailure)
            return user.Error;

        var hasActive = await _subscriptionRepository
            .HasActiveSubscriptionAsync(command.Request.UserId, cancellationToken);
        if (hasActive)
            return GeneralErrors.AlreadyExists("Активная подписка");

        var plan = await _planRepository
            .GetByIdAsync(command.Request.PlanId, cancellationToken);
        if (plan.IsFailure)
            return plan.Error;

        if (!plan.Value.IsActive)
            return GeneralErrors.InvalidOperation("Нельзя подписаться на неактивный план.");

        if (command.Request.WithTrial)
        {
            var markTrialResult = user.Value.MarkTrialUsed();
            if (markTrialResult.IsFailure)
                return markTrialResult.Error;
        }

        var subscription = Subscription.Create(
            Guid.NewGuid(),
            command.Request.UserId,
            command.Request.PlanId,
            Guid.NewGuid(),
            plan.Value.Price,
            plan.Value.BillingPeriod,
            command.Request.WithTrial,
            _dateTime.UtcNow);

        _subscriptionRepository.Add(subscription);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        return new CreateSubscriptionResponse(subscription.Id);
    }
}
