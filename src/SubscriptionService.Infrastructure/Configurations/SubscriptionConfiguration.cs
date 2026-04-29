using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionService.Domain.Aggregates.Subscription;

namespace SubscriptionService.Infrastructure.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("subscriptions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.PlanId)
            .HasColumnName("plan_id")
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.CurrentPeriodEnd)
            .HasColumnName("current_period_end")
            .IsRequired();

        builder.Property(s => s.CancelAtPeriodEnd)
            .HasColumnName("cancel_at_period_end")
            .IsRequired();

        builder.Property(s => s.TrialEnd)
            .HasColumnName("trial_end");

        builder.Property(s => s.CreatedWhen)
            .HasColumnName("created_when")
            .IsRequired();

        builder.Property(s => s.CancelledWhen)
            .HasColumnName("cancelled_when");

        builder.Property(s => s.ExpiredWhen)
            .HasColumnName("expired_when");

        builder.OwnsMany(s => s.Invoices, invoiceBuilder =>
        {
            invoiceBuilder.ToTable("invoices");

            invoiceBuilder.HasKey(i => i.Id);
            invoiceBuilder.Property(i => i.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            invoiceBuilder.OwnsOne(i => i.Amount, amountBuilder =>
            {
                amountBuilder.Property(a => a.Value)
                    .HasColumnName("amount")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            invoiceBuilder.Property(i => i.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            invoiceBuilder.Property(i => i.DueDate)
                .HasColumnName("due_date")
                .IsRequired();

            invoiceBuilder.Property(i => i.CreatedWhen)
                .HasColumnName("created_when")
                .IsRequired();

            invoiceBuilder.Property(i => i.PaidWhen)
                .HasColumnName("paid_when");
        });

        builder.Navigation(s => s.Invoices)
            .HasField("_invoices");

        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("ix_subscriptions_user_id");
    }
}
