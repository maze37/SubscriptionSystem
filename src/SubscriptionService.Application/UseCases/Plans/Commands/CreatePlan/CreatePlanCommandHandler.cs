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
    public async Task<Result<Guid>> Handle(
        CreatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = Plan.Create(
            Guid.NewGuid(),
            command.Name,
            command.Price,
            command.BillingPeriod,
            _dateTime.UtcNow);

        _planRepository.Add(plan);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<Guid>.Success(plan.Id);
    }
}