using SubscriptionService.Domain.Aggregates.Plan;

namespace SubscriptionService.Application.Abstractions;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default);
    void Add(Plan plan);
}