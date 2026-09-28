using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Domain.Aggregates.Plan;

namespace SubscriptionService.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для чтения и записи тарифных планов.
/// </summary>
public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _context;

    public PlanRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Result<Plan, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await _context.Plans
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (plan is null)
            return GeneralErrors.NotFound(id, "План");

        return plan;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<Plan>, Error>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var allActivePlans = await _context.Plans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price.Value)
            .ToListAsync(cancellationToken);

        return allActivePlans;
    }

    /// <inheritdoc/>
    public void Add(Plan plan)
    {
        _context.Plans.Add(plan);
    }
}
