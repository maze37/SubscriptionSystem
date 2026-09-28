using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.User;

/// <summary>
/// Агрегат пользователя.
/// Хранит минимальные данные необходимые для управления подпиской.
/// </summary>
public class User
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Версия агрегата для оптимистичной блокировки.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public UserEmail Email { get; private set; } = null!;

    /// <summary>
    /// Использовал ли пользователь триальный период.
    /// </summary>
    public bool HasUsedTrial { get; private set; }

    /// <summary>
    /// Дата регистрации.
    /// </summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>
    /// Для EF Core.
    /// </summary>
    private User() { }

    private User(
        Guid id,
        UserEmail email,
        DateTimeOffset createdWhen)
    {
        Id = id;
        Email = email;
        HasUsedTrial = false;
        CreatedWhen = createdWhen;
    }

    /// <summary>
    /// Зарегистрировать нового пользователя.
    /// </summary>
    public static User Create(
        Guid userId,
        UserEmail email,
        DateTimeOffset createdWhen)
    {
        return new User(
            userId,
            email,
            createdWhen);
    }

    /// <summary>
    /// Отметить что пользователь использовал триал.
    /// Триальный период можно использовать только один раз.
    /// </summary>
    public UnitResult<Error> MarkTrialUsed()
    {
        if (HasUsedTrial)
            return GeneralErrors.InvalidOperation("Триальный период уже был использован.");

        HasUsedTrial = true;
        return UnitResult.Success<Error>();
    }
}
