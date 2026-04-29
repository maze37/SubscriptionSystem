using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Infrastructure.Repositories;

namespace SubscriptionService.Infrastructure;

/// <summary>
/// Регистрация инфраструктурного слоя в DI.
/// Подключает EF Core, репозитории и системные провайдеры.
/// </summary>
public static class Inject
{
    /// <summary>Добавить инфраструктурные зависимости приложения.</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("PostgreSQL")
                               ?? throw new InvalidOperationException("Connection String не найдено.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                npgsql.CommandTimeout(30);
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
