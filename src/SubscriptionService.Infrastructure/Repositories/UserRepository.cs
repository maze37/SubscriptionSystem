using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates.User;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с агрегатом User
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Result<User, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
            return GeneralErrors.NotFound(id, "Пользователь");

        return user;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByEmailAsync(UserEmail email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    /// <inheritdoc/>
    public void Add(User user)
    {
        _context.Users.Add(user);
    }
}
