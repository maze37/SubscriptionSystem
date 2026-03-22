using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.Plan;

/// <summary>
/// Агрегат тарифного плана.
/// Справочник доступных планов подписки.
/// </summary>
public class Plan : AggregateRoot
{
    /// <summary>Название плана.</summary>
    public PlanName Name { get; private set; } = null!;

    /// <summary>Цена плана.</summary>
    public Money Price { get; private set; } = null!;

    /// <summary>Период оплаты — месяц или год.</summary>
    public BillingPeriod BillingPeriod { get; private set; }

    /// <summary>Активен ли план — можно ли на него подписаться.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Дата создания плана.</summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>Для EF Core.</summary>
    private Plan() : base(Guid.Empty) { }

    private Plan(
        Guid id,
        PlanName name,
        Money price,
        BillingPeriod billingPeriod,
        DateTimeOffset createdWhen) : base(id)
    {
        Name = name;
        Price = price;
        BillingPeriod = billingPeriod;
        IsActive = true;
        CreatedWhen = createdWhen;
    }

    /// <summary>Создать новый тарифный план.</summary>
    public static Plan Create(
        Guid planId,
        string name,
        decimal price,
        BillingPeriod billingPeriod,
        DateTimeOffset createdWhen)
    {
        if (planId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Plan.InvalidId,
                "ID плана не может быть пустым.",
                nameof(planId));

        var planNameResult = PlanName.Create(name);
        var priceResult = Money.Create(price);
        
        return new Plan(
            planId,
            planNameResult,
            priceResult,
            billingPeriod,
            createdWhen);
    }

    /// <summary>
    /// Деактивировать план — новые подписки невозможны.
    /// Существующие подписки продолжают работать.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException(
                DomainErrors.Plan.AlreadyDeactivated,
                "План уже деактивирован.");

        IsActive = false;
    }

    /// <summary>Активировать план.</summary>
    public void Activate()
    {
        if (IsActive)
            throw new DomainException(
                DomainErrors.Plan.AlreadyActive,
                "План уже активен.");

        IsActive = true;
    }
}