using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SubscriptionService.Domain.Aggregates.Plan;
using SubscriptionService.Domain.Aggregates.Subscription;
using SubscriptionService.Domain.Aggregates.User;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.ValueObjects;
using SubscriptionService.Infrastructure;
using SubscriptionService.Infrastructure.Database;
using Xunit;

namespace SubscriptionService.Tests;

public class PostgresConcurrencyTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 9, 30, 0, 0, 0, TimeSpan.Zero);

    [PostgresFact]
    public async Task StaleSubscriptionVersionReturnsConcurrencyConflict()
    {
        var ids = await SeedActiveSubscriptionAsync();

        try
        {
            await using var firstContext = CreateContext();
            await using var secondContext = CreateContext();

            var first = await firstContext.Subscriptions
                .Include(s => s.Invoices)
                .SingleAsync(s => s.Id == ids.SubscriptionId);
            var second = await secondContext.Subscriptions
                .Include(s => s.Invoices)
                .SingleAsync(s => s.Id == ids.SubscriptionId);

            Assert.True(first.Cancel(Now.AddDays(1)).IsSuccess);
            Assert.True(second.Expire(Now.AddDays(1)).IsSuccess);

            var firstManager = CreateTransactionManager(firstContext);
            var secondManager = CreateTransactionManager(secondContext);

            Assert.True((await firstManager.SaveChangesAsync(CancellationToken.None)).IsSuccess);
            Assert.True((await secondManager.SaveChangesAsync(CancellationToken.None)).IsFailure);
        }
        finally
        {
            await CleanupAsync(ids);
        }
    }

    [PostgresFact]
    public async Task UniqueIndexRejectsTwoCurrentSubscriptionsForOneUser()
    {
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();

        await using (var setupContext = CreateContext())
        {
            setupContext.Users.Add(CreateUser(userId));
            setupContext.Plans.Add(CreatePlan(planId));
            await setupContext.SaveChangesAsync();
        }

        var firstSubscriptionId = Guid.NewGuid();
        var secondSubscriptionId = Guid.NewGuid();

        try
        {
            await using var firstContext = CreateContext();
            await using var secondContext = CreateContext();

            firstContext.Subscriptions.Add(CreateSubscription(firstSubscriptionId, userId, planId));
            secondContext.Subscriptions.Add(CreateSubscription(secondSubscriptionId, userId, planId));

            Assert.True((await CreateTransactionManager(firstContext)
                .SaveChangesAsync(CancellationToken.None)).IsSuccess);

            Assert.True((await CreateTransactionManager(secondContext)
                .SaveChangesAsync(CancellationToken.None)).IsFailure);
        }
        finally
        {
            await CleanupAsync((userId, planId, firstSubscriptionId));
            await using var cleanupContext = CreateContext();
            await cleanupContext.Subscriptions
                .Where(s => s.Id == secondSubscriptionId)
                .ExecuteDeleteAsync();
        }
    }

    private static AppDbContext CreateContext()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            PostgresFactAttribute.ConnectionStringVariable)!;
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    private static TransactionManager CreateTransactionManager(AppDbContext context)
    {
        return new TransactionManager(
            context,
            NullLogger<TransactionManager>.Instance,
            NullLoggerFactory.Instance);
    }

    private static async Task<(Guid UserId, Guid PlanId, Guid SubscriptionId)>
        SeedActiveSubscriptionAsync()
    {
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscription = CreateSubscription(subscriptionId, userId, planId);
        var invoice = Assert.Single(subscription.Invoices);
        Assert.True(subscription.Activate(invoice.Id, Now).IsSuccess);

        await using var context = CreateContext();
        context.Users.Add(CreateUser(userId));
        context.Plans.Add(CreatePlan(planId));
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();

        return (userId, planId, subscriptionId);
    }

    private static User CreateUser(Guid userId)
    {
        return User.Create(
            userId,
            UserEmail.Create($"{userId:N}@example.com").Value,
            Now);
    }

    private static Plan CreatePlan(Guid planId)
    {
        return Plan.Create(
            planId,
            PlanName.Create($"Plan-{planId:N}").Value,
            Money.Create(100).Value,
            BillingPeriod.Monthly,
            Now);
    }

    private static Subscription CreateSubscription(
        Guid subscriptionId,
        Guid userId,
        Guid planId)
    {
        return Subscription.Create(
            subscriptionId,
            userId,
            planId,
            Guid.NewGuid(),
            Money.Create(100).Value,
            BillingPeriod.Monthly,
            false,
            Now);
    }

    private static async Task CleanupAsync(
        (Guid UserId, Guid PlanId, Guid SubscriptionId) ids)
    {
        await using var context = CreateContext();
        await context.Subscriptions
            .Where(s => s.Id == ids.SubscriptionId)
            .ExecuteDeleteAsync();
        await context.Users
            .Where(u => u.Id == ids.UserId)
            .ExecuteDeleteAsync();
        await context.Plans
            .Where(p => p.Id == ids.PlanId)
            .ExecuteDeleteAsync();
    }
}
