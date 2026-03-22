using Microsoft.EntityFrameworkCore;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates.Subscription;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Subscriptions
            .Include(s => s.Invoices)
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            .ConfigureAwait(false);
    }

    public async Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Subscriptions
            .Include(s => s.Invoices)
            .FirstOrDefaultAsync(s =>
                s.UserId == userId &&
                (s.Status == SubscriptionStatus.Active ||
                 s.Status == SubscriptionStatus.Trial), ct)
            .ConfigureAwait(false);
    }

    public async Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Subscriptions
            .AnyAsync(s =>
                s.UserId == userId &&
                (s.Status == SubscriptionStatus.Active ||
                 s.Status == SubscriptionStatus.Trial), ct)
            .ConfigureAwait(false);
    }

    public void Add(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        _context.Subscriptions.Add(subscription);
    }

    public void Update(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        _context.Subscriptions.Update(subscription);
    }
}