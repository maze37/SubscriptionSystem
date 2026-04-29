using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;

namespace SubscriptionService.Domain.ValueObjects;

/// <summary>
/// Value Object — денежная сумма.
/// Инвариант: должна быть больше нуля.
/// </summary>
public class Money : ValueObject
{
    public decimal Value { get; }
    
    private Money(decimal value) => Value = value;

    /// <summary>Создать денежную сумму с валидацией.</summary>
    public static Result<Money, Error> Create(decimal value)
    {
        if (value <= 0)
            return Result<Money, Error>.Failure(Error.Validation(
                DomainErrors.Money.InvalidAmount,
                "Цена должна быть больше нуля.",
                nameof(value)));

        return Result<Money, Error>.Success(new Money(value));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator decimal(Money money) => money.Value;
}
