using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Aggregates.Plan;
using SubscriptionService.Domain.Aggregates.Subscription;
using SubscriptionService.Domain.Aggregates.User;

namespace SubscriptionService.Infrastructure;

/// <summary>
/// EF Core контекст приложения.
/// Описывает набор агрегатов, которые хранятся в базе данных.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>Пользователи.</summary>
    public DbSet<User> Users => Set<User>();
    /// <summary>Тарифные планы.</summary>
    public DbSet<Plan> Plans => Set<Plan>();
    /// <summary>Подписки.</summary>
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    
    /// <summary>Применяет все конфигурации сущностей из сборки Infrastructure.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder, nameof(modelBuilder));
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
