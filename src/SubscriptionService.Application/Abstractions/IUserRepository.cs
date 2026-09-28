using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.Aggregates.User;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Application.Abstractions;

/// <summary>
/// Репозиторий пользователей.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получить пользователя по ID или ошибку, если пользователь не найден.
    /// </summary>
    Task<Result<User, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить существует ли пользователь с указанным email.
    /// </summary>
    Task<bool> ExistsByEmailAsync(UserEmail email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить нового пользователя.
    /// </summary>
    void Add(User user);
}
