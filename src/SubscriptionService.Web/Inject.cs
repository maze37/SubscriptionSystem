using SubscriptionService.Application;
using SubscriptionService.Infrastructure;

namespace SubscriptionService.Web;

public static class Inject
{
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