using Microsoft.EntityFrameworkCore;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates.Plan;

namespace SubscriptionService.Infrastructure.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _context;

    public PlanRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Plans
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.Plans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public void Add(Plan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        _context.Plans.Add(plan);
    }
}