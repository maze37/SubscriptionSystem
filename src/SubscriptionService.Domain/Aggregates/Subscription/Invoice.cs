using SharedKernel.Base;
using SharedKernel.Constants;
using SharedKernel.Result;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates.Subscription;

/// <summary>
/// Счёт на оплату подписки.
/// Entity внутри агрегата Subscription — не существует без подписки.
/// </summary>
public class Invoice : Entity
{
    /// <summary>Сумма к оплате.</summary>
    public Money Amount { get; private set; } = null!;

    /// <summary>Статус счёта.</summary>
    public InvoiceStatus Status { get; private set; }
    
    /// <summary>Срок оплаты.</summary>
    public DateTimeOffset DueDate { get; private set; }

    /// <summary>Дата создания счёта.</summary>
    public DateTimeOffset CreatedWhen { get; private set; }

    /// <summary>Дата оплаты (null если не оплачен).</summary>
    public DateTimeOffset? PaidWhen { get; private set; }

    /// <summary>Для EF Core.</summary>
    private Invoice() : base(Guid.Empty) { }

    private Invoice(
        Guid id,
        Money amount,
        DateTimeOffset dueDate,
        DateTimeOffset createdWhen) : base(id)
    {
        Amount = amount;
        Status = InvoiceStatus.Pending;
        DueDate = dueDate;
        CreatedWhen = createdWhen;
    }

    /// <summary>Создать новый счёт на оплату.</summary>
    public static Result<Invoice, Error> Create(
        Guid invoiceId,
        Money amount,
        DateTimeOffset dueDate,
        DateTimeOffset createdWhen)
    {
        if (invoiceId == Guid.Empty)
            return Result<Invoice, Error>.Failure(Error.Validation(
                DomainErrors.Invoice.InvalidId,
                "ID счёта не может быть пустым.",
                nameof(invoiceId)));

        var moneyResult = Money.Create(amount);
        if (moneyResult.IsFailure)
            return Result<Invoice, Error>.Failure(moneyResult.Error!);

        return Result<Invoice, Error>.Success(new Invoice(
            invoiceId,
            moneyResult.Value!,
            dueDate, 
            createdWhen));
    }

    /// <summary>Отметить счёт как оплаченный.</summary>
    public Result<Error> MarkAsPaid(DateTimeOffset paidWhen)
    {
        if (Status == InvoiceStatus.Paid)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Invoice.AlreadyPaid,
                "Счёт уже оплачен."));

        if (Status == InvoiceStatus.Failed)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Invoice.AlreadyFailed,
                "Нельзя оплатить отклонённый счёт."));

        Status = InvoiceStatus.Paid;
        PaidWhen = paidWhen;
        return Result<Error>.Success();
    }

    /// <summary>Отметить счёт как неоплаченный (платёж отклонён).</summary>
    public Result<Error> MarkAsFailed()
    {
        if (Status == InvoiceStatus.Paid)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Invoice.AlreadyPaid,
                "Нельзя отклонить уже оплаченный счёт."));

        if (Status == InvoiceStatus.Failed)
            return Result<Error>.Failure(Error.Conflict(
                DomainErrors.Invoice.AlreadyFailed,
                "Счёт уже отклонён."));

        Status = InvoiceStatus.Failed;
        return Result<Error>.Success();
    }
}
