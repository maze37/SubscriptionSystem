using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.Subscription;

/// <summary>
/// Агрегат подписки пользователя на тарифный план.
/// Управляет жизненным циклом подписки и счетами на оплату.
/// </summary>
public class Subscription : AggregateRoot
{
    /// <summary>Длительность триального периода в днях.</summary>
    private const int TrialDurationDays = 14;

    /// <summary>ID пользователя которому принадлежит подписка.</summary>
    public Guid UserId { get; private set; }

    /// <summary>ID тарифного плана.</summary>
    public Guid PlanId { get; private set; }

    /// <summary>Текущий статус подписки.</summary>
    public SubscriptionStatus Status { get; private set; }

    /// <summary>Конец текущего оплаченного периода.</summary>
    public DateTimeOffset CurrentPeriodEnd { get; private set; }

    /// <summary>Отменить подписку в конце периода а не сразу.</summary>
    public bool CancelAtPeriodEnd { get; private set; }

    /// <summary>Дата окончания триала (null если без триала).</summary>
    public DateTimeOffset? TrialEnd { get; private set; }

    /// <summary>Дата создания подписки.</summary>
    public DateTimeOffset CreatedWhen { get; private set; }
    
    /// <summary>Дата отмены.</summary>
    public DateTimeOffset? CancelledWhen { get; private set; }
    
    /// <summary>Дата окончания подписки.</summary>
    public DateTimeOffset? ExpiredWhen { get; private set; }

    private readonly List<Invoice> _invoices = [];
    /// <summary>Список счетов на оплату.</summary>
    public IReadOnlyList<Invoice> Invoices => _invoices.AsReadOnly();

    /// <summary>Для EF Core.</summary>
    private Subscription() : base(Guid.Empty) { }

    private Subscription(
        Guid id,
        Guid userId,
        Guid planId,
        SubscriptionStatus status,
        DateTimeOffset currentPeriodEnd,
        DateTimeOffset createdWhen,
        DateTimeOffset? trialEnd = null) : base(id)
    {
        UserId = userId;
        PlanId = planId;
        Status = status;
        CurrentPeriodEnd = currentPeriodEnd;
        TrialEnd = trialEnd;
        CancelAtPeriodEnd = false;
        CreatedWhen = createdWhen;
    }

    private static Result<DateTimeOffset, Error> CalculatePeriodEnd(
        DateTimeOffset startedWhen,
        BillingPeriod billingPeriod) =>
        billingPeriod switch
        {
            BillingPeriod.Monthly => Result<DateTimeOffset, Error>.Success(startedWhen.AddMonths(1)),
            BillingPeriod.Yearly => Result<DateTimeOffset, Error>.Success(startedWhen.AddYears(1)),
            _ => Result<DateTimeOffset, Error>.Failure(Error.Validation(
                DomainErrors.Plan.InvalidBillingPeriod,
                $"Неподдерживаемый billing period '{billingPeriod}'.",
                nameof(billingPeriod)))
        };

    /// <summary>
    /// Создать новую подписку.
    /// Если withTrial = true — подписка начинается с триального периода (14 дней).
    /// Если withTrial = false — сразу создаётся счёт на оплату.
    /// </summary>
    public static Result<Subscription, Error> Create(
        Guid id,
        Guid userId,
        Guid planId,
        Guid invoiceId,
        Money price,
        BillingPeriod billingPeriod,
        bool withTrial,
        DateTimeOffset createdWhen)
    {
        if (id == Guid.Empty)
            return Result<Subscription, Error>.Failure(Error.Validation(
                DomainErrors.Subscription.InvalidId,
                "ID подписки не может быть пустым.",
                nameof(id)));

        if (userId == Guid.Empty)
            return Result<Subscription, Error>.Failure(Error.Validation(
                DomainErrors.Subscription.InvalidUserId,
                "ID пользователя не может быть пустым.",
                nameof(userId)));

        if (planId == Guid.Empty)
            return Result<Subscription, Error>.Failure(Error.Validation(
                DomainErrors.Subscription.InvalidPlanId,
                "ID плана не может быть пустым.",
                nameof(planId)));
        
        if (withTrial)
        {
            var trialEnd = createdWhen.AddDays(TrialDurationDays);

            return Result<Subscription, Error>.Success(new Subscription(
                id,
                userId,
                planId,
                SubscriptionStatus.Trial,
                currentPeriodEnd: trialEnd,
                createdWhen: createdWhen,
                trialEnd: trialEnd));
        }
        
        if (invoiceId == Guid.Empty)
            return Result<Subscription, Error>.Failure(Error.Validation(
                DomainErrors.Invoice.InvalidId,
                "ID счета не может быть пустым.",
                nameof(invoiceId)));

        var periodEndResult = CalculatePeriodEnd(createdWhen, billingPeriod);
        if (periodEndResult.IsFailure)
            return Result<Subscription, Error>.Failure(periodEndResult.Error!);

        var subscription = new Subscription(
            id,
            userId,
            planId,
            SubscriptionStatus.Active,
            currentPeriodEnd: periodEndResult.Value!,
            createdWhen: createdWhen);

        var invoice = Invoice.Create(
            invoiceId,
            price,
            dueDate: createdWhen.AddDays(3),
            createdWhen: createdWhen);

        if (invoice.IsFailure)
            return Result<Subscription, Error>.Failure(invoice.Error!);

        subscription._invoices.Add(invoice.Value!);

        return Result<Subscription, Error>.Success(subscription);
    }

    /// <summary>
    /// Отменить подписку.
    /// Доступ сохраняется до конца текущего периода.
    /// </summary>
    public Result<Error> Cancel(DateTimeOffset cancelledWhen)
    {
        if (Status == SubscriptionStatus.Cancelled)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Subscription.AlreadyCancelled,
                "Подписка уже отменена."));

        if (Status == SubscriptionStatus.Expired)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Subscription.AlreadyExpired,
                "Нельзя отменить истекшую подписку."));

        CancelledWhen = cancelledWhen;
        CancelAtPeriodEnd = true;
        return Result<Error>.Success();
    }

    /// <summary>
    /// Сменить тарифный план.
    /// Создаётся новый счёт на оплату по цене нового плана.
    /// </summary>
    public Result<Error> ChangePlan(
        Guid invoiceId,
        Guid newPlanId,
        Money newPrice,
        DateTimeOffset changedWhen)
    {
        if (Status != SubscriptionStatus.Active &&
            Status != SubscriptionStatus.Trial)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Subscription.CannotChangePlan,
                "Нельзя сменить план неактивной подписки."));
        
        if (newPlanId == Guid.Empty)
            return Result<Error>.Failure(Error.Validation(
                DomainErrors.Subscription.InvalidPlanId,
                "ID нового плана не может быть пустым.",
                nameof(newPlanId)));
        
        if (invoiceId == Guid.Empty)
            return Result<Error>.Failure(Error.Validation(
                DomainErrors.Invoice.InvalidId,
                "ID счета не может быть пустым.",
                nameof(invoiceId)));

        PlanId = newPlanId;

        var invoice = Invoice.Create(
            invoiceId,
            newPrice,
            dueDate: changedWhen.AddDays(3),
            createdWhen: changedWhen);

        if (invoice.IsFailure)
            return Result<Error>.Failure(invoice.Error!);

        _invoices.Add(invoice.Value!);
        return Result<Error>.Success();
    }

    /// <summary>
    /// Продлить подписку на следующий период.
    /// Вызывается фоновым сервисом когда заканчивается CurrentPeriodEnd.
    /// Если CancelAtPeriodEnd = true — подписка переходит в Cancelled.
    /// </summary>
    public Result<Error> Renew(
        Guid invoiceId,
        Money price,
        BillingPeriod billingPeriod,
        DateTimeOffset renewedWhen)
    {
        if (CancelAtPeriodEnd)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Subscription.CannotRenew,
                "Подписка запланирована к отмене в конце периода и не может быть продлена."));

        if (Status != SubscriptionStatus.Active)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Subscription.CannotRenew,
                "Нельзя продлить неактивную подписку."));
        
        if (invoiceId == Guid.Empty)
            return Result<Error>.Failure(Error.Validation(
                DomainErrors.Invoice.InvalidId,
                "ID счёта не может быть пустым.",
                nameof(invoiceId)));
        
        var periodEndResult = CalculatePeriodEnd(renewedWhen, billingPeriod);
        if (periodEndResult.IsFailure)
            return Result<Error>.Failure(periodEndResult.Error!);

        CurrentPeriodEnd = periodEndResult.Value!;
        
        var invoice = Invoice.Create(
            invoiceId,
            price,
            dueDate: renewedWhen.AddDays(3),
            createdWhen: renewedWhen);

        if (invoice.IsFailure)
            return Result<Error>.Failure(invoice.Error!);

        _invoices.Add(invoice.Value!);
        return Result<Error>.Success();
    }

    /// <summary>
    /// Активировать подписку после оплаты счёта.
    /// </summary>
    public Result<Error> Activate(
        Guid invoiceId,
        BillingPeriod billingPeriod,
        DateTimeOffset activatedWhen)
    {
        var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId);

        if (invoice is null)
            return Result<Error>.Failure(Error.NotFound(
                DomainErrors.Invoice.NotFound,
                $"Счёт с ID '{invoiceId}' не найден."));

        var invoicePaymentResult = invoice.MarkAsPaid(activatedWhen);
        if (invoicePaymentResult.IsFailure)
            return invoicePaymentResult;

        var periodEndResult = CalculatePeriodEnd(activatedWhen, billingPeriod);
        if (periodEndResult.IsFailure)
            return Result<Error>.Failure(periodEndResult.Error!);

        Status = SubscriptionStatus.Active;
        CurrentPeriodEnd = periodEndResult.Value!;
        return Result<Error>.Success();
    }

    /// <summary>
    /// Истечь подписку — вызывается если счёт не оплачен вовремя.
    /// </summary>
    public Result<Error> Expire(DateTimeOffset expiredWhen)
    {
        if (Status == SubscriptionStatus.Expired)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Subscription.AlreadyExpired,
                "Подписка уже истекла."));

        ExpiredWhen = expiredWhen;
        Status = SubscriptionStatus.Expired;
        return Result<Error>.Success();
    }
}
