using SubscriptionService.Domain.Aggregates.Subscription;

namespace SubscriptionService.Application.Abstractions;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken ct = default);
    void Add(Subscription subscription);
    void Update(Subscription subscription);
}