using SubscriptionService.Application;
using SubscriptionService.Infrastructure;

namespace SubscriptionService.Web;

/// <summary>
/// Точка регистрации всех зависимостей веб-приложения.
/// </summary>
public static class Inject
{
    /// <summary>Подключает Application, Infrastructure и API-сервисы.</summary>
    public static IServiceCollection ConfigureApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddControllers();

        return services;
    }
}
