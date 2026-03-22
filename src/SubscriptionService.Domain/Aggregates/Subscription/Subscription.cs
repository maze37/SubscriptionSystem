using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Exceptions;
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

    /// <summary>
    /// Создать новую подписку.
    /// Если withTrial = true — подписка начинается с триального периода (14 дней).
    /// Если withTrial = false — сразу создаётся счёт на оплату.
    /// </summary>
    public static Subscription Create(
        Guid id,
        Guid userId,
        Guid planId,
        Guid invoiceId,
        Money price,
        bool withTrial,
        DateTimeOffset createdWhen)
    {
        if (id == Guid.Empty)
            throw new DomainException(
                DomainErrors.Subscription.InvalidId,
                "ID подписки не может быть пустым.",
                nameof(id));

        if (userId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Subscription.InvalidUserId,
                "ID пользователя не может быть пустым.",
                nameof(userId));

        if (planId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Subscription.InvalidPlanId,
                "ID плана не может быть пустым.",
                nameof(planId));
        
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
        
        if (invoiceId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Invoice.InvalidId,
                "ID счета не может быть пустым.",
                nameof(invoiceId));

        var subscription = new Subscription(
            id,
            userId,
            planId,
            SubscriptionStatus.Active,
            currentPeriodEnd: createdWhen.AddMonths(1),
            createdWhen: createdWhen);

        var invoice = Invoice.Create(
            invoiceId,
            price,
            dueDate: createdWhen.AddDays(3),
            createdWhen: createdWhen);

        subscription._invoices.Add(invoice);

        return subscription;
    }

    /// <summary>
    /// Отменить подписку.
    /// Доступ сохраняется до конца текущего периода.
    /// </summary>
    public void Cancel(DateTimeOffset cancelledWhen)
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new DomainException(
                DomainErrors.Subscription.AlreadyCancelled,
                "Подписка уже отменена.");

        if (Status == SubscriptionStatus.Expired)
            throw new DomainException(
                DomainErrors.Subscription.AlreadyExpired,
                "Нельзя отменить истекшую подписку.");

        CancelledWhen = cancelledWhen;
        CancelAtPeriodEnd = true;
    }

    /// <summary>
    /// Сменить тарифный план.
    /// Создаётся новый счёт на оплату по цене нового плана.
    /// </summary>
    public void ChangePlan(
        Guid invoiceId,
        Guid newPlanId,
        Money newPrice,
        DateTimeOffset changedWhen)
    {
        if (Status != SubscriptionStatus.Active &&
            Status != SubscriptionStatus.Trial)
            throw new DomainException(
                DomainErrors.Subscription.CannotChangePlan,
                "Нельзя сменить план неактивной подписки.");
        
        if (newPlanId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Subscription.InvalidPlanId,
                "ID нового плана не может быть пустым.",
                nameof(newPlanId));
        
        if (invoiceId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Invoice.InvalidId,
                "ID счета не может быть пустым.",
                nameof(invoiceId));

        PlanId = newPlanId;

        var invoice = Invoice.Create(
            invoiceId,
            newPrice,
            dueDate: changedWhen.AddDays(3),
            createdWhen: changedWhen);

        _invoices.Add(invoice);
    }

    /// <summary>
    /// Продлить подписку на следующий период.
    /// Вызывается фоновым сервисом когда заканчивается CurrentPeriodEnd.
    /// Если CancelAtPeriodEnd = true — подписка переходит в Cancelled.
    /// </summary>
    public void Renew(
        Guid invoiceId,
        Money price,
        DateTimeOffset renewedWhen)
    {
        if (CancelAtPeriodEnd)
        {
            Status = SubscriptionStatus.Cancelled;
            return;
        }

        if (Status != SubscriptionStatus.Active)
            throw new DomainException(
                DomainErrors.Subscription.CannotRenew,
                "Нельзя продлить неактивную подписку.");
        
        if (invoiceId == Guid.Empty)
            throw new DomainException(
                DomainErrors.Invoice.InvalidId,
                "ID счёта не может быть пустым.",
                nameof(invoiceId));
        
        CurrentPeriodEnd = renewedWhen.AddMonths(1);
        
        var invoice = Invoice.Create(
            invoiceId,
            price,
            dueDate: renewedWhen.AddDays(3),
            createdWhen: renewedWhen);

        _invoices.Add(invoice);
    }

    /// <summary>
    /// Активировать подписку после оплаты счёта.
    /// </summary>
    public void Activate(Guid invoiceId, DateTimeOffset activatedWhen)
    {
        var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId);

        if (invoice is null)
            throw new DomainException(
                DomainErrors.Invoice.NotFound,
                $"Счёт с ID '{invoiceId}' не найден.");

        invoice.MarkAsPaid(activatedWhen);

        Status = SubscriptionStatus.Active;
        CurrentPeriodEnd = activatedWhen.AddMonths(1);
    }

    /// <summary>
    /// Истечь подписку — вызывается если счёт не оплачен вовремя.
    /// </summary>
    public void Expire(DateTimeOffset expiredWhen)
    {
        if (Status == SubscriptionStatus.Expired)
            throw new DomainException(
                DomainErrors.Subscription.AlreadyExpired,
                "Подписка уже истекла.");

        ExpiredWhen = expiredWhen;
        Status = SubscriptionStatus.Expired;
    }
}