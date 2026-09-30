using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.Subscription;

/// <summary>
/// Агрегат подписки пользователя на тарифный план.
/// Управляет жизненным циклом подписки и счетами на оплату.
/// </summary>
public class Subscription : IVersionedEntity
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Версия агрегата для оптимистичной блокировки.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Длительность триального периода в днях.
    /// </summary>
    private const int TrialDurationDays = 14;

    /// <summary>
    /// ID пользователя которому принадлежит подписка.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ID тарифного плана.
    /// </summary>
    public Guid PlanId { get; private set; }

    /// <summary>
    /// Текущий статус подписки.
    /// </summary>
    public SubscriptionStatus Status { get; private set; }

    /// <summary>
    /// Конец текущего оплаченного периода.
    /// </summary>
    public DateTimeOffset CurrentPeriodEnd { get; private set; }

    /// <summary>
    /// Отменить подписку в конце периода а не сразу.
    /// </summary>
    public bool CancelAtPeriodEnd { get; private set; }

    /// <summary>
    /// Дата окончания триала (null если без триала).
    /// </summary>
    public DateTimeOffset? TrialEnd { get; private set; }

    /// <summary>
    /// Дата создания подписки.
    /// </summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>
    /// Дата отмены.
    /// </summary>
    public DateTimeOffset? CancelledWhen { get; private set; }

    /// <summary>
    /// Дата окончания подписки.
    /// </summary>
    public DateTimeOffset? ExpiredWhen { get; private set; }

    /// <summary>
    /// Список счетов на оплату.
    /// </summary>
    private readonly List<Invoice> _invoices = [];
    
    public IReadOnlyList<Invoice> Invoices => _invoices.AsReadOnly();

    /// <summary>
    /// Для EF Core.
    /// </summary>
    private Subscription()  { }

    private Subscription(
        Guid id,
        Guid userId,
        Guid planId,
        SubscriptionStatus status,
        DateTimeOffset currentPeriodEnd,
        DateTimeOffset createdWhen,
        DateTimeOffset? trialEnd = null)
    {
        Id = id;
        UserId = userId;
        PlanId = planId;
        Status = status;
        CurrentPeriodEnd = currentPeriodEnd;
        TrialEnd = trialEnd;
        CancelAtPeriodEnd = false;
        CreatedWhen = createdWhen;
    }
    
    /// <summary>
    /// Создать новую подписку.
    /// Если withTrial = true - подписка начинается с триального периода (14 дней).
    /// Если withTrial = false - сразу создаётся счёт на оплату.
    /// </summary>
    public static Subscription Create(
        Guid id,
        Guid userId,
        Guid planId,
        Guid invoiceId,
        Money price,
        BillingPeriod billingPeriod,
        bool withTrial,
        DateTimeOffset createdWhen)
    {
        if (withTrial)
        {
            var trialEnd = createdWhen.AddDays(TrialDurationDays);

            return new Subscription(
                id,
                userId,
                planId,
                SubscriptionStatus.Trial,
                currentPeriodEnd: trialEnd,
                createdWhen: createdWhen,
                trialEnd: trialEnd);
        }

        var subscription = new Subscription(
            id,
            userId,
            planId,
            SubscriptionStatus.PendingPayment,
            currentPeriodEnd: createdWhen,
            createdWhen: createdWhen);

        var invoice = Invoice.Create(
            invoiceId,
            price,
            InvoicePurpose.InitialSubscription,
            planId,
            billingPeriod,
            dueDate: createdWhen.AddDays(3),
            createdWhen: createdWhen);

        subscription._invoices.Add(invoice);

        return subscription;
    }
    
    private static DateTimeOffset CalculatePeriodEnd(
        DateTimeOffset startedWhen,
        BillingPeriod billingPeriod) =>
        billingPeriod switch
        {
            BillingPeriod.Monthly => startedWhen.AddMonths(1),
            BillingPeriod.Yearly => startedWhen.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(billingPeriod), billingPeriod, null)
        };

    /// <summary>
    /// Отменить подписку.
    /// Доступ сохраняется до конца текущего периода.
    /// </summary>
    public UnitResult<Error> Cancel(DateTimeOffset cancelledWhen)
    {
        if (Status == SubscriptionStatus.Cancelled || CancelAtPeriodEnd)
            return GeneralErrors.InvalidOperation("Подписка уже отменена.");

        if (Status == SubscriptionStatus.Expired)
            return GeneralErrors.InvalidOperation("Нельзя отменить истекшую подписку.");

        CancelledWhen = cancelledWhen;
        CancelAtPeriodEnd = true;
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Сменить тарифный план.
    /// Создаётся новый счёт на оплату по цене нового плана.
    /// </summary>
    public UnitResult<Error> ChangePlan(
        Guid invoiceId,
        Guid newPlanId,
        Money newPrice,
        BillingPeriod billingPeriod,
        DateTimeOffset changedWhen)
    {
        if (Status != SubscriptionStatus.Active &&
            Status != SubscriptionStatus.Trial)
            return GeneralErrors.InvalidOperation("Нельзя сменить план неактивной подписки.");

        if (_invoices.Any(invoice => invoice.Status == InvoiceStatus.Pending))
            return GeneralErrors.InvalidOperation("У подписки уже есть неоплаченный счёт.");

        var invoice = Invoice.Create(
            invoiceId,
            newPrice,
            InvoicePurpose.ChangePlan,
            newPlanId,
            billingPeriod,
            dueDate: changedWhen.AddDays(3),
            createdWhen: changedWhen);

        _invoices.Add(invoice);
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Создать счёт на продление следующего периода.
    /// Период изменится только после оплаты счёта.
    /// </summary>
    public UnitResult<Error> Renew(
        Guid invoiceId,
        Money price,
        BillingPeriod billingPeriod,
        DateTimeOffset renewedWhen)
    {
        if (CancelAtPeriodEnd)
            return GeneralErrors.InvalidOperation("Подписка запланирована к отмене в конце периода и не может быть продлена.");

        if (Status != SubscriptionStatus.Active)
            return GeneralErrors.InvalidOperation("Нельзя продлить неактивную подписку.");

        if (_invoices.Any(invoice => invoice.Status == InvoiceStatus.Pending))
            return GeneralErrors.InvalidOperation("У подписки уже есть неоплаченный счёт.");

        var invoice = Invoice.Create(
            invoiceId,
            price,
            InvoicePurpose.Renewal,
            PlanId,
            billingPeriod,
            dueDate: renewedWhen.AddDays(3),
            createdWhen: renewedWhen);

        _invoices.Add(invoice);
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Активировать подписку после оплаты счёта.
    /// </summary>
    public UnitResult<Error> Activate(
        Guid invoiceId,
        DateTimeOffset activatedWhen)
    {
        var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId);

        if (invoice is null)
            return GeneralErrors.NotFound(invoiceId, nameof(Invoice));

        if (invoice.Status == InvoiceStatus.Paid)
            return UnitResult.Success<Error>();

        var paymentResult = invoice.MarkAsPaid(activatedWhen);
        if (paymentResult.IsFailure)
            return paymentResult;

        switch (invoice.Purpose)
        {
            case InvoicePurpose.InitialSubscription:
            case InvoicePurpose.ChangePlan:
                PlanId = invoice.PlanId;
                Status = SubscriptionStatus.Active;
                CurrentPeriodEnd = CalculatePeriodEnd(activatedWhen, invoice.BillingPeriod);
                break;

            case InvoicePurpose.Renewal:
                var periodStart = CurrentPeriodEnd > activatedWhen
                    ? CurrentPeriodEnd
                    : activatedWhen;

                Status = SubscriptionStatus.Active;
                CurrentPeriodEnd = CalculatePeriodEnd(periodStart, invoice.BillingPeriod);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(invoice.Purpose));
        }

        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Истечь подписку - вызывается если счёт не оплачен вовремя.
    /// </summary>
    public UnitResult<Error> Expire(DateTimeOffset expiredWhen)
    {
        if (Status == SubscriptionStatus.Expired)
            return GeneralErrors.InvalidOperation("Подписка уже истекла.");

        ExpiredWhen = expiredWhen;
        Status = SubscriptionStatus.Expired;
        return UnitResult.Success<Error>();
    }

    public void IncreaseVersion()
    {
        Version++;
    }
}
