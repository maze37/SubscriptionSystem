using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Aggregates.Plan;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Infrastructure.Configurations;

/// <summary>
/// Конфигурация EF Core для агрегата Plan.
/// </summary>
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.Version)
            .HasColumnName("Version")
            .IsConcurrencyToken()
            .IsRequired();

        builder.ComplexProperty(p => p.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("name")
                .HasMaxLength(PlanName.MaxLength)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(p => p.Value)
                .HasColumnName("price")
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.Property(p => p.BillingPeriod)
            .HasColumnName("billing_period")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(p => p.CreatedWhen)
            .HasColumnName("created_when")
            .IsRequired();
    }
}
