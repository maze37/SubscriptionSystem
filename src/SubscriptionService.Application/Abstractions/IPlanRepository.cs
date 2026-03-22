using SubscriptionService.Domain.Aggregates.Plan;

namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Репозиторий тарифных планов.
/// Все методы записи не сохраняют изменения — вызывай IUnitOfWork.SaveChangesAsync.
/// </summary>
public interface IPlanRepository
{
    /// <summary>Получить план по ID. Возвращает null если не найден.</summary>
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Получить все активные планы отсортированные по цене.</summary>
    Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default);

    /// <summary>Добавить новый план. Требует SaveChangesAsync.</summary>
    void Add(Plan plan);
}