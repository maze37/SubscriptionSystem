using SubscriptionService.Application.Abstractions;

namespace SubscriptionService.Infrastructure;

/// <summary>
/// Реализация единицы работы через EF Core.
/// Сохраняет все изменения отслеживаемых агрегатов одной транзакцией.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context ??  throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct)
            .ConfigureAwait(false);
    }
}