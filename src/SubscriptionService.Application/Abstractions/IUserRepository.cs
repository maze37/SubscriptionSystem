using SubscriptionService.Domain.Aggregates.User;

namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Репозиторий пользователей.
/// Все методы записи не сохраняют изменения — вызывай IUnitOfWork.SaveChangesAsync.
/// </summary>
public interface IUserRepository
{
    /// <summary>Получить пользователя по ID. Возвращает null если не найден.</summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Проверить существует ли пользователь с указанным email.</summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Добавить нового пользователя. Требует SaveChangesAsync.</summary>
    void Add(User user);
}