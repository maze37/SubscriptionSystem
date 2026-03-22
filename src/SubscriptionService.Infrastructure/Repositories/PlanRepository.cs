using Microsoft.EntityFrameworkCore;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates.Plan;

namespace SubscriptionService.Infrastructure.Repositories;

/// Репозиторий для работы с агрегатом Plan
public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _context;

    public PlanRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Plans
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.Plans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    public void Add(Plan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        _context.Plans.Add(plan);
    }
}