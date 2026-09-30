using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.Aggregates.Subscription;

namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Репозиторий подписок.
/// Методы записи не сохраняют изменения самостоятельно.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    /// Получить подписку по ID вместе со счетами или ошибку, если подписка не найдена.
    /// </summary>
    Task<Result<Subscription, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить есть ли у пользователя активная подписка.
    /// </summary>
    Task<bool> HasActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую подписку.
    /// </summary>
    void Add(Subscription subscription);

    /// <summary>
    /// Обновить подписку.
    /// </summary>
    void Update(Subscription subscription);
}
