using SubscriptionService.Domain.Aggregates.Subscription;

namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Репозиторий подписок.
/// Все методы записи не сохраняют изменения — вызывай IUnitOfWork.SaveChangesAsync.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>Получить подписку по ID включая Invoice. Возвращает null если не найдена.</summary>
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Получить активную подписку пользователя (Trial или Active).</summary>
    Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Проверить есть ли у пользователя активная подписка.</summary>
    Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Добавить новую подписку. Требует SaveChangesAsync.</summary>
    void Add(Subscription subscription);

    /// <summary>Обновить подписку. Требует SaveChangesAsync.</summary>
    void Update(Subscription subscription);
}