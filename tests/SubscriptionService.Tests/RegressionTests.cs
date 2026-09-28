using SubscriptionService.Domain.Aggregates.Subscription;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;
using Xunit;

namespace SubscriptionService.Tests;

public class RegressionTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 28, 0, 0, 0, TimeSpan.Zero);
    private static Subscription Create() => Subscription.Create(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        Money.Create(100).Value, BillingPeriod.Monthly, false, Now);

    [Fact]
    public void PaidInvoiceCannotBePaidTwice()
    {
        var subscription = Create();
        var invoice = Assert.Single(subscription.Invoices);
        Assert.True(subscription.Activate(invoice.Id, BillingPeriod.Monthly, Now).IsSuccess);
        Assert.True(subscription.Activate(invoice.Id, BillingPeriod.Monthly, Now).IsFailure);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(Now, invoice.PaidWhen);
    }

    [Fact]
    public void FailedPlanChangeDoesNotMutateSubscription()
    {
        var subscription = Create();
        var planId = subscription.PlanId;
        Assert.True(subscription.Expire(Now).IsSuccess);
        Assert.True(subscription.ChangePlan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Money.Create(100).Value,
            Now).IsFailure);
        Assert.Equal(planId, subscription.PlanId);
        Assert.Single(subscription.Invoices);
    }

    [Fact]
    public void FailedRenewalDoesNotChangePeriod()
    {
        var subscription = Create();
        var periodEnd = subscription.CurrentPeriodEnd;
        Assert.True(subscription.Cancel(Now).IsSuccess);
        Assert.True(subscription.Renew(
            Guid.NewGuid(),
            Money.Create(100).Value,
            BillingPeriod.Yearly,
            Now).IsFailure);
        Assert.Equal(periodEnd, subscription.CurrentPeriodEnd);
        Assert.Single(subscription.Invoices);
    }

    [Fact]
    public void ValueObjectsRejectInvalidInput()
    {
        Assert.True(Money.Create(0).IsFailure);
        Assert.True(PlanName.Create(string.Empty).IsFailure);
        Assert.True(UserEmail.Create("invalid-email").IsFailure);
    }

    [Fact]
    public void RepeatedCancellationPreservesTimestamp()
    {
        var subscription = Create();
        Assert.True(subscription.Cancel(Now).IsSuccess);
        Assert.True(subscription.Cancel(Now.AddDays(1)).IsFailure);
        Assert.Equal(Now, subscription.CancelledWhen);
    }

    [Fact]
    public void EmailIsNormalizedAndLengthIsChecked()
    {
        Assert.Equal("user@example.com", UserEmail.Create(" USER@example.com ").Value.Value);
        Assert.True(UserEmail.Create(new string('a', 256) + "@example.com").IsFailure);
    }

}
