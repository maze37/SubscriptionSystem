using SubscriptionService.Application.Abstractions;

namespace SubscriptionService.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ??  throw new ArgumentNullException(nameof(context));
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct)
            .ConfigureAwait(false);
    }
}