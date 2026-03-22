using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace SubscriptionService.Domain.ValueObjects;

/// <summary>
/// Value Object — денежная сумма.
/// Инвариант: должна быть больше нуля.
/// </summary>
public class Money : ValueObject
{
    public decimal Value { get; }
    
    private Money(decimal value) => Value = value;

    /// <summary>
    /// Создать денежную сумму с валидацией.
    /// </summary>
    /// <param name="value">Сумма.</param>
    /// <exception cref="DomainException">Если сумма меньше или равна нулю.</exception>
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