namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Единица работы — сохраняет все изменения в БД одной транзакцией.
/// Вызывается в конце каждого хендлера после всех изменений агрегатов.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Сохранить все изменения в базе данных.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}