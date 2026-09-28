using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application;
using SubscriptionService.Infrastructure;

namespace SubscriptionService.Web.Configuration;

/// <summary>
/// Точка регистрации всех зависимостей веб-приложения.
/// </summary>
public static class Inject
{
    /// <summary>
    /// Подключает Application, Infrastructure и API-сервисы.
    /// </summary>
    public static IServiceCollection ConfigureApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddControllers();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
}
