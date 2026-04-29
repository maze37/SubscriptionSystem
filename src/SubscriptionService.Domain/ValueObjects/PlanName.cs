using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;

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

    /// <summary>Создать название плана с валидацией.</summary>
    public static Result<PlanName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<PlanName, Error>.Failure(Error.Validation(
                DomainErrors.PlanName.Empty,
                "Название плана не может быть пустым.",
                nameof(value)));

        if (value.Length > MaxLength)
            return Result<PlanName, Error>.Failure(Error.Validation(
                DomainErrors.PlanName.TooLong,
                "Название плана слишком длинное",
                nameof(value)));

        return Result<PlanName, Error>.Success(new PlanName(value));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator string(PlanName value) => value.Value;
}
