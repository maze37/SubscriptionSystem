using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace SubscriptionService.Domain.ValueObjects;

/// <summary>
/// Value Object — название тарифного плана.
/// Инварианты: не может быть пустым, максимум 100 символов.
/// </summary>
public class PlanName : ValueObject
{
    /// <summary>Максимальная длина названия плана.</summary>
    public const int MaxLength = 100;
    public string Value { get; }
    
    private PlanName(string value) => Value = value;

    /// <summary>
    /// Создать название плана с валидацией.
    /// </summary>
    /// <param name="value">Название плана.</param>
    /// <exception cref="DomainException">Если название пустое или превышает максимальную длину.</exception>

    public static PlanName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                DomainErrors.PlanName.Empty,
                "Название плана не может быть пустым.",
                nameof(value));

        if (value.Length > MaxLength)
            throw new DomainException(
                DomainErrors.PlanName.TooLong,
                "Название плана слишком длинное",
                nameof(value));

        return new PlanName(value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator string(PlanName value) => value.Value;
}