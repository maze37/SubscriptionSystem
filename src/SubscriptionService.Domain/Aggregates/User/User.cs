using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;
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
    public static Result<User, Error> Create(
        Guid userId,
        string email,
        DateTimeOffset createdWhen)
    {
        if (userId == Guid.Empty)
            return Result<User, Error>.Failure(Error.Validation(
                DomainErrors.User.InvalidId,
                "ID пользователя не может быть пустым.",
                nameof(userId)));

        var emailResult = UserEmail.Create(email);
        if (emailResult.IsFailure)
            return Result<User, Error>.Failure(emailResult.Error!);

        return Result<User, Error>.Success(new User(
            userId,
            emailResult.Value!,
            createdWhen));
    }

    /// <summary>
    /// Отметить что пользователь использовал триал.
    /// Триальный период можно использовать только один раз.
    /// </summary>
    public Result<Error> MarkTrialUsed()
    {
        if (HasUsedTrial)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.User.TrialAlreadyUsed,
                "Триальный период уже был использован."));

        HasUsedTrial = true;
        return Result<Error>.Success();
    }
}
