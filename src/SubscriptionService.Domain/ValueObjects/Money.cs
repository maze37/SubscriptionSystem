using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace SubscriptionService.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Value { get; }
    
    private Money(decimal value) => Value = value;

    public static Money Create(decimal value)
    {
        if (value <= 0)
            throw new DomainException(
                DomainErrors.Money.InvalidAmount,
                "Цена должна быть больше нуля.",
                nameof(value));
        
        return new Money(value);

    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator decimal(Money money) => money.Value;
}