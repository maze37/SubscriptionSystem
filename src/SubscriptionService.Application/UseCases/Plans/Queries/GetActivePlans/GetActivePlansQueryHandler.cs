using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;

/// <summary>
/// Обработчик запроса GetActivePlansQuery.
/// Возвращает все активные планы отсортированные по цене.
/// </summary>
public class GetActivePlansQueryHandler
    : IQueryHandler<GetActivePlansQuery, IReadOnlyList<PlanResponse>>
{
    private readonly IPlanRepository _planRepository;

    public GetActivePlansQueryHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository
                          ?? throw new ArgumentNullException(nameof(planRepository));
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<PlanResponse>, Error>> Handle(
        GetActivePlansQuery query,
        CancellationToken cancellationToken)
    {
        var plans = await _planRepository
            .GetAllActiveAsync(cancellationToken)
            .ConfigureAwait(false);

        var response = plans
            .Select(p => p.ToResponse())
            .ToList();

        return Result<IReadOnlyList<PlanResponse>, Error>.Success(response);
    }
}
