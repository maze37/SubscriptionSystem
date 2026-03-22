using Microsoft.EntityFrameworkCore;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates;
using SubscriptionService.Domain.Aggregates.User;

namespace SubscriptionService.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с агрегатом User
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.Value == email, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _context.Users.Add(user);
    }
}