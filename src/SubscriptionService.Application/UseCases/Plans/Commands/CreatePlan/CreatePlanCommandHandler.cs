using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Domain.Aggregates.Plan;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, Guid>
{
    private readonly IPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlanCommandHandler(
        IPlanRepository planRepository,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid>> Handle(
        CreatePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = Plan.Create(
            command.Id,
            command.Name,
            command.Price,
            command.BillingPeriod,
            command.CreatedWhen);

        _planRepository.Add(plan);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Result<Guid>.Success(plan.Id);
    }
}