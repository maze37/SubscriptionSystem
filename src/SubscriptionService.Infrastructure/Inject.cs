using Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Infrastructure.Database;
using SubscriptionService.Infrastructure.Repositories;

namespace SubscriptionService.Infrastructure;

/// <summary>
/// Регистрация инфраструктурного слоя в DI.
/// Подключает EF Core, репозитории и системные провайдеры.
/// </summary>
public static class Inject
{
    /// <summary>
    /// Добавить инфраструктурные зависимости приложения.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SubscriptionSystemDb")
                               ?? throw new InvalidOperationException(
                                   "Строка подключения SubscriptionSystemDb не найдена.");

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(connectionString);
            options.UseLoggerFactory(loggerFactory);
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
