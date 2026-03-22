using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;

public record GetActivePlansQuery : IQuery<IReadOnlyList<PlanResponse>>;