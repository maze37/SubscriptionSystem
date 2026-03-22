using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace SubscriptionService.Domain.ValueObjects;

public class PlanName : ValueObject
{
    public const int MaxLength = 100;
    public string Value { get; }
    
    private PlanName(string value) => Value = value;

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