using Core.Database;
using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Aggregates.Plan;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

/// <summary>
/// Обработчик команды CreatePlanCommand.
/// Создаёт тарифный план и сохраняет в БД.
/// </summary>
public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, CreatePlanResponse>
{
    private readonly IPlanRepository _planRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IDateTimeProvider _dateTime;

    public CreatePlanCommandHandler(
        IPlanRepository planRepository,
        ITransactionManager transactionManager,
        IDateTimeProvider dateTime)
    {
        _planRepository = planRepository;
        _transactionManager = transactionManager;
        _dateTime = dateTime;
    }

    /// <inheritdoc/>
    public async Task<Result<CreatePlanResponse, Error>> HandleAsync(
        CreatePlanCommand command,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(command.Request.BillingPeriod))
            return GeneralErrors.ValueIsInvalid(nameof(command.Request.BillingPeriod));

        var nameResult = PlanName.Create(command.Request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var priceResult = Money.Create(command.Request.Price);
        if (priceResult.IsFailure)
            return priceResult.Error;

        var plan = Plan.Create(
            Guid.NewGuid(),
            nameResult.Value,
            priceResult.Value,
            command.Request.BillingPeriod,
            _dateTime.UtcNow);

        _planRepository.Add(plan);

        var saveResult = await _transactionManager
            .SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        return new CreatePlanResponse(plan.Id);
    }
}
