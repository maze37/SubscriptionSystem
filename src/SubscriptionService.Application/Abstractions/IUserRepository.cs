using SubscriptionService.Domain.Aggregates.User;

namespace SubscriptionService.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    void Add(User user);
}