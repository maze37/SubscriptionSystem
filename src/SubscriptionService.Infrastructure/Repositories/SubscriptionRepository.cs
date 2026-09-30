using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates.Subscription;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для чтения и записи подписок.
/// </summary>
public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Result<Subscription, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Invoices)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (subscription is null)
            return GeneralErrors.NotFound(id, "Подписка");

        return subscription;
    }

    /// <inheritdoc/>
    public async Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Subscriptions
            .AnyAsync(s =>
                s.UserId == userId &&
                (s.Status == SubscriptionStatus.PendingPayment ||
                 s.Status == SubscriptionStatus.Active ||
                 s.Status == SubscriptionStatus.Trial ||
                 s.Status == SubscriptionStatus.PastDue), cancellationToken);
    }

    /// <inheritdoc/>
    public void Add(Subscription subscription)
    {
        _context.Subscriptions.Add(subscription);
    }

    /// <inheritdoc/>
    public void Update(Subscription subscription)
    {
        var entry = _context.Entry(subscription);
        if (entry.State == EntityState.Detached)
            _context.Subscriptions.Attach(subscription);
    }
}
