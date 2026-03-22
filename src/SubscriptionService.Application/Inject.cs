using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionService.Application.Behaviors;

namespace SubscriptionService.Application;

public static class Inject
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Inject).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        // Находим всех наследников AbstractValidator
        services.AddValidatorsFromAssembly(typeof(Inject).Assembly);

        return services;
    }
}