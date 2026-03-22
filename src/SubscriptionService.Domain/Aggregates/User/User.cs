using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.User;

/// <summary>
/// Агрегат пользователя.
/// Хранит минимальные данные необходимые для управления подпиской.
/// </summary>
public class User : AggregateRoot
{
    /// <summary>Email пользователя.</summary>
    public UserEmail Email { get; private set; } = null!;

    /// <summary>Использовал ли пользователь триальный период.</summary>
    public bool HasUsedTrial { get; private set; }

    /// <summary>Дата регистрации.</summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>Для EF Core.</summary>
    private User() : base(Guid.Empty) { }

    private User(
        Guid id,
        UserEmail email,
        DateTimeOffset createdWhen) : base(id)
    {
        Email = email;
        HasUsedTrial = false;
        CreatedWhen = createdWhen;
    }

    /// <summary>Зарегистрировать нового пользователя.</summary>
    public static User Create(
        Guid userId,
        string email,
        DateTimeOffset createdWhen)
    {
        if (userId == Guid.Empty)
            throw new DomainException(
                DomainErrors.User.InvalidId,
                "ID пользователя не может быть пустым.",
                nameof(userId));

        return new User(
            userId,
            UserEmail.Create(email),
            createdWhen);
    }

    /// <summary>
    /// Отметить что пользователь использовал триал.
    /// Триальный период можно использовать только один раз.
    /// </summary>
    public void MarkTrialUsed()
    {
        if (HasUsedTrial)
            throw new DomainException(
                DomainErrors.User.TrialAlreadyUsed,
                "Триальный период уже был использован.");

        HasUsedTrial = true;
    }
}