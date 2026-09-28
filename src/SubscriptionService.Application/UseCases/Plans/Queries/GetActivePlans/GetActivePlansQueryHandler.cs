using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;

/// <summary>
/// Обработчик запроса GetActivePlansQuery.
/// Возвращает все активные планы отсортированные по цене.
/// </summary>
public class GetActivePlansQueryHandler
    : IQueryHandlerWithResult<GetActivePlansQuery, GetActivePlansResponse>
{
    private readonly IPlanRepository _planRepository;

    public GetActivePlansQueryHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    /// <inheritdoc/>
    public async Task<Result<GetActivePlansResponse, Error>> HandleAsync(
        GetActivePlansQuery query,
        CancellationToken cancellationToken)
    {
        var plansResult = await _planRepository
            .GetAllActiveAsync(cancellationToken);

        if (plansResult.IsFailure)
            return plansResult.Error;

        var response = plansResult.Value
            .Select(plan => new PlanResponse(
                plan.Id,
                plan.Name,
                plan.Price,
                plan.BillingPeriod,
                plan.IsActive))
            .ToList();

        return new GetActivePlansResponse(response);
    }
}
