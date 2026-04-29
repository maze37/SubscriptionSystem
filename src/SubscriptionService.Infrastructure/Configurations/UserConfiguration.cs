using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionService.Domain.Aggregates.User;
using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Infrastructure.Configurations;

/// <summary>
/// Конфигурация EF Core для агрегата User.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>Настройка таблицы и колонок пользователя.</summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");

        builder.ComplexProperty(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(UserEmail.MaxLenght)
                .IsRequired();
        });

        builder.Property(u => u.HasUsedTrial)
            .HasColumnName("has_used_trial")
            .IsRequired();

        builder.Property(u => u.CreatedWhen)
            .HasColumnName("created_when")
            .IsRequired();
    }
}
