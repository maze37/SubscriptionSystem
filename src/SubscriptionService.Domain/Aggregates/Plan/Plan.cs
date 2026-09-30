using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.Plan;

/// <summary>
/// Агрегат тарифного плана.
/// Справочник доступных планов подписки.
/// </summary>
public class Plan : IVersionedEntity
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Версия агрегата для оптимистичной блокировки.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Название плана.
    /// </summary>
    public PlanName Name { get; private set; } = null!;

    /// <summary>
    /// Цена плана.
    /// </summary>
    public Money Price { get; private set; } = null!;

    /// <summary>
    /// Период оплаты (месяц или год).
    /// </summary>
    public BillingPeriod BillingPeriod { get; private set; }

    /// <summary>
    /// Активен ли план (можно ли на него подписаться).
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Дата создания плана.
    /// </summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>
    /// Для EF Core.
    /// </summary>
    private Plan() { }

    private Plan(
        Guid id,
        PlanName name,
        Money price,
        BillingPeriod billingPeriod,
        DateTimeOffset createdWhen)
    {
        Id = id;
        Name = name;
        Price = price;
        BillingPeriod = billingPeriod;
        IsActive = true;
        CreatedWhen = createdWhen;
    }

    /// <summary>
    /// Создать новый тарифный план.
    /// </summary>
    public static Plan Create(
        Guid planId,
        PlanName name,
        Money price,
        BillingPeriod billingPeriod,
        DateTimeOffset createdWhen)
    {
        return new Plan(
            planId,
            name,
            price,
            billingPeriod,
            createdWhen);
    }

    /// <summary>
    /// Деактивировать план (новые подписки невозможны).
    /// Существующие подписки продолжают работать.
    /// </summary>
    public UnitResult<Error> Deactivate()
    {
        if (!IsActive)
            return GeneralErrors.InvalidOperation("План уже деактивирован.");

        IsActive = false;
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Активировать план.
    /// </summary>
    public UnitResult<Error> Activate()
    {
        if (IsActive)
            return GeneralErrors.InvalidOperation("План уже активен.");

        IsActive = true;
        return UnitResult.Success<Error>();
    }

    public void IncreaseVersion()
    {
        Version++;
    }
}
