using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.Aggregates.Plan;

namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Репозиторий тарифных планов.
/// Методы записи не сохраняют изменения самостоятельно.
/// </summary>
public interface IPlanRepository
{
    /// <summary>
    /// Получить план по ID или ошибку, если план не найден.
    /// </summary>
    Task<Result<Plan, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все активные планы, отсортированные по цене.
    /// </summary>
    Task<Result<IReadOnlyList<Plan>, Error>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новый план.
    /// </summary>
    void Add(Plan plan);
}
