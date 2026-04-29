namespace SubscriptionService.Application.Abstractions;

using SharedKernel.Result;

/// <summary>
/// Единица работы — сохраняет все изменения в БД одной транзакцией.
/// Вызывается в конце каждого хендлера после всех изменений агрегатов.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Сохранить все изменения в базе данных.</summary>
    Task<Result<Error>> SaveChangesAsync(CancellationToken ct = default);
}
