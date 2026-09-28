using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using SharedKernel;

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

    /// <summary>
    /// Максимальная длина email.
    /// </summary>
    public const int MAX_LENGTH = 255;

    public string Value { get; }

    private UserEmail(string value) => Value = value;

    /// <summary>
    /// Создать email с валидацией формата.
    /// </summary>
    public static Result<UserEmail, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsInvalid(nameof(value), "Email не может быть пустым.");

        value = value.Trim();
        if (value.Length > MAX_LENGTH)
            return GeneralErrors.LengthIsInvalid(nameof(value), max: MAX_LENGTH);

        if (!EmailRegex.IsMatch(value))
            return GeneralErrors.ValueIsInvalid(nameof(value), "Некорректный формат email.");

        return new UserEmail(value.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(UserEmail email) => email.Value;
}
