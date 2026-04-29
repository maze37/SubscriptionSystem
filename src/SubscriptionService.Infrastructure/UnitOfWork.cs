using Microsoft.EntityFrameworkCore;
using SubscriptionService.Application.Abstractions;
using SharedKernel.Result;

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
    public async Task<Result<Error>> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await _context.SaveChangesAsync(ct)
                .ConfigureAwait(false);
            return Result<Error>.Success();
        }
        catch (DbUpdateException ex)
        {
            return Result<Error>.Failure(
                Error.Conflict(
                    "db.update.conflict",
                    $"Конфликт сохранения данных: {ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result<Error>.Failure(
                Error.Failure(
                    "db.save.failed",
                    $"Ошибка сохранения данных: {ex.Message}"));
        }
    }
}
