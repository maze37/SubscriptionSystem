using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Aggregates.Plan;
using SubscriptionService.Domain.Aggregates.Subscription;
using SubscriptionService.Domain.Aggregates.User;

namespace SubscriptionService.Infrastructure;

/// <summary>
/// EF Core контекст приложения.
/// Описывает набор агрегатов, которые хранятся в базе данных.
/// </summary>
public class AppDbContext : DbContext
{
    private const string DesignTimeConnectionString =
        "Host=localhost;Port=25434;Database=subscription_system_db;Username=postgres;Password=1234";

    /// <summary>
    /// Создаёт контекст для инструментов EF Core.
    /// </summary>
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Пользователи.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Тарифные планы.
    /// </summary>
    public DbSet<Plan> Plans => Set<Plan>();

    /// <summary>
    /// Подписки.
    /// </summary>
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SubscriptionSystemDb")
                               ?? DesignTimeConnectionString;

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
