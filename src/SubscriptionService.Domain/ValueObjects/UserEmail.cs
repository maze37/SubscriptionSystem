using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;
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

    /// <summary>
    /// Создать email с валидацией формата.
    /// </summary>
    /// <param name="value">Строка email.</param>
    /// <exception cref="DomainException">Если email пустой или некорректного формата.</exception>

    public static UserEmail Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                DomainErrors.UserEmail.Empty,
                "Email не может быть пустым.",
                nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new DomainException(
                DomainErrors.UserEmail.Invalid,
                "Некорректный формат email.",
                nameof(value));

        return new UserEmail(value.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(UserEmail email) => email.Value;
}