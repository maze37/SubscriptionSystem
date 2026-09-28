using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.Subscription;

/// <summary>
/// Счёт на оплату подписки.
/// Entity внутри агрегата Subscription - не существует без подписки.
/// </summary>
public class Invoice
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Сумма к оплате.
    /// </summary>
    public Money Amount { get; private set; } = null!;

    /// <summary>
    /// Статус счёта.
    /// </summary>
    public InvoiceStatus Status { get; private set; }

    /// <summary>
    /// Срок оплаты.
    /// </summary>
    public DateTimeOffset DueDate { get; private set; }

    /// <summary>
    /// Дата создания счёта.
    /// </summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>
    /// Дата оплаты (null если не оплачен).
    /// </summary>
    public DateTimeOffset? PaidWhen { get; private set; }

    /// <summary>
    /// Для EF Core.
    /// </summary>
    private Invoice()  { }

    private Invoice(
        Guid id,
        Money amount,
        DateTimeOffset dueDate,
        DateTimeOffset createdWhen)
    {
        Id = id;
        Amount = amount;
        Status = InvoiceStatus.Pending;
        DueDate = dueDate;
        CreatedWhen = createdWhen;
    }

    /// <summary>
    /// Создать новый счёт на оплату.
    /// </summary>
    public static Invoice Create(
        Guid invoiceId,
        Money amount,
        DateTimeOffset dueDate,
        DateTimeOffset createdWhen)
    {
        return new Invoice(
            invoiceId,
            amount,
            dueDate,
            createdWhen);
    }

    /// <summary>
    /// Отметить счёт как оплаченный.
    /// </summary>
    public UnitResult<Error> MarkAsPaid(DateTimeOffset paidWhen)
    {
        if (Status == InvoiceStatus.Paid)
            return GeneralErrors.InvalidOperation("Счёт уже оплачен.");

        if (Status == InvoiceStatus.Failed)
            return GeneralErrors.InvalidOperation("Нельзя оплатить отклонённый счёт.");

        Status = InvoiceStatus.Paid;
        PaidWhen = paidWhen;
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Отметить счёт как неоплаченный (платёж отклонён).
    /// </summary>
    public UnitResult<Error> MarkAsFailed()
    {
        if (Status == InvoiceStatus.Paid)
            return GeneralErrors.InvalidOperation("Нельзя отклонить уже оплаченный счёт.");

        if (Status == InvoiceStatus.Failed)
            return GeneralErrors.InvalidOperation("Счёт уже отклонён.");

        Status = InvoiceStatus.Failed;
        return UnitResult.Success<Error>();
    }
}
