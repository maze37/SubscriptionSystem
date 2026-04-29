// CreatePlanCommandHandler.cs
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Domain.Aggregates.Plan;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

/// <summary>
/// Обработчик команды CreatePlanCommand.
/// Создаёт тарифный план и сохраняет в БД.
/// </summary>
public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, Guid>
{
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTime;

    public CreatePlanCommandHandler(
        IPlanRepository planRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTime)
    {
        _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    /// <inheritdoc/>
    public async Task<Result<Guid, Error>> Handle(
        CreatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = Plan.Create(
            Guid.NewGuid(),
            command.Name,
            command.Price,
            command.BillingPeriod,
            _dateTime.UtcNow);
        if (plan.IsFailure)
            return Result<Guid, Error>.Failure(plan.Error!);

        _planRepository.Add(plan.Value!);

        var saveResult = await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        if (saveResult.IsFailure)
            return Result<Guid, Error>.Failure(saveResult.Error!);

        return Result<Guid, Error>.Success(plan.Value!.Id);
    }
}
