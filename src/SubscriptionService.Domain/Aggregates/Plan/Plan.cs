using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;
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
    public static Result<Plan, Error> Create(
        Guid planId,
        string name,
        decimal price,
        BillingPeriod billingPeriod,
        DateTimeOffset createdWhen)
    {
        if (planId == Guid.Empty)
            return Result<Plan, Error>.Failure(Error.Validation(
                DomainErrors.Plan.InvalidId,
                "ID плана не может быть пустым.",
                nameof(planId)));

        if (!Enum.IsDefined(billingPeriod))
            return Result<Plan, Error>.Failure(Error.Validation(
                DomainErrors.Plan.InvalidBillingPeriod,
                $"Неподдерживаемый billing period '{billingPeriod}'.",
                nameof(billingPeriod)));

        var planNameResult = PlanName.Create(name);
        if (planNameResult.IsFailure)
            return Result<Plan, Error>.Failure(planNameResult.Error!);

        var priceResult = Money.Create(price);
        if (priceResult.IsFailure)
            return Result<Plan, Error>.Failure(priceResult.Error!);

        return Result<Plan, Error>.Success(new Plan(
            planId,
            planNameResult.Value!,
            priceResult.Value!,
            billingPeriod,
            createdWhen));
    }

    /// <summary>
    /// Деактивировать план — новые подписки невозможны.
    /// Существующие подписки продолжают работать.
    /// </summary>
    public Result<Error> Deactivate()
    {
        if (!IsActive)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Plan.AlreadyDeactivated,
                "План уже деактивирован."));

        IsActive = false;
        return Result<Error>.Success();
    }

    /// <summary>Активировать план.</summary>
    public Result<Error> Activate()
    {
        if (IsActive)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Plan.AlreadyActive,
                "План уже активен."));

        IsActive = true;
        return Result<Error>.Success();
    }
}
