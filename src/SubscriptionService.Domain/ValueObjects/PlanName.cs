using CSharpFunctionalExtensions;
using SharedKernel;
namespace SubscriptionService.Domain.ValueObjects;

/// <summary>
/// Value Object — название тарифного плана.
/// Инварианты: не может быть пустым, максимум 100 символов.
/// </summary>
public class PlanName : ValueObject
{
    /// <summary>
    /// Максимальная длина названия плана.
    /// </summary>
    public const int MaxLength = 100;
    public string Value { get; }

    private PlanName(string value) => Value = value;

    /// <summary>
    /// Создать название плана с валидацией.
    /// </summary>
    public static Result<PlanName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsInvalid(nameof(value), "Название плана не может быть пустым.");

        if (value.Length > MaxLength)
            return GeneralErrors.ValueIsInvalid(nameof(value), "Название плана слишком длинное");

        return new PlanName(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(PlanName value) => value.Value;
}
