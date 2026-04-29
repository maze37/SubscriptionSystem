using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;
using System.Text.RegularExpressions;

namespace SubscriptionService.Domain.ValueObjects;

/// <summary>
/// Value Object - email пользователя.
/// Инварианты: не может быть пустым, должен соответствовать формату email.
/// Хранится в нижнем регистре.
/// </summary>
public class UserEmail : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>Максимальная длина email.</summary>
    public const int MaxLenght = 255;
    
    public string Value { get; }

    private UserEmail(string value) => Value = value;

    /// <summary>Создать email с валидацией формата.</summary>
    public static Result<UserEmail, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<UserEmail, Error>.Failure(Error.Validation(
                DomainErrors.UserEmail.Empty,
                "Email не может быть пустым.",
                nameof(value)));

        if (!EmailRegex.IsMatch(value))
            return Result<UserEmail, Error>.Failure(Error.Validation(
                DomainErrors.UserEmail.Invalid,
                "Некорректный формат email.",
                nameof(value)));

        return Result<UserEmail, Error>.Success(new UserEmail(value.Trim().ToLowerInvariant()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(UserEmail email) => email.Value;
}
