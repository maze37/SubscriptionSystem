using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;

public class GetActivePlansQueryHandler
    : IQueryHandler<GetActivePlansQuery, IReadOnlyList<PlanResponse>>
{
    private readonly IPlanRepository _planRepository;

    public GetActivePlansQueryHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository
                          ?? throw new ArgumentNullException(nameof(planRepository));
    }

    public async Task<Result<IReadOnlyList<PlanResponse>>> Handle(
        GetActivePlansQuery query,
        CancellationToken cancellationToken)
    {
        var plans = await _planRepository
            .GetAllActiveAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = plans
            .Select(p => new PlanResponse(
                p.Id,
                p.Name,
                p.Price,
                p.BillingPeriod,
                p.IsActive))
            .ToList();

        return Result<IReadOnlyList<PlanResponse>>.Success(response);
    }
}