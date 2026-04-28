using SubscriptionService.Domain.Aggregates.Plan;
using SubscriptionService.Domain.Aggregates.Subscription;

namespace SubscriptionService.Application.DTOs;

public static class DtoMappings
{
    public static PlanResponse ToResponse(this Plan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        return new PlanResponse(
            plan.Id,
            plan.Name,
            plan.Price,
            plan.BillingPeriod,
            plan.IsActive);
    }

    public static SubscriptionResponse ToResponse(this Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        return new SubscriptionResponse(
            subscription.Id,
            subscription.UserId,
            subscription.PlanId,
            subscription.Status,
            subscription.CurrentPeriodEnd,
            subscription.CancelAtPeriodEnd,
            subscription.TrialEnd,
            subscription.CreatedWhen,
            subscription.Invoices.Select(ToResponse).ToList());
    }

    public static InvoiceResponse ToResponse(this Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        return new InvoiceResponse(
            invoice.Id,
            invoice.Amount.Value,
            invoice.Status,
            invoice.DueDate,
            invoice.CreatedWhen,
            invoice.PaidWhen);
    }
}
