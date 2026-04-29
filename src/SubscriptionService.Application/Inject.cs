using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionService.Application.Behaviors;

namespace SubscriptionService.Application;

/// <summary>
/// Регистрация сервисов слоя Application.
/// Подключает MediatR, handlers и FluentValidation pipeline.
/// </summary>
public static class Inject
{
    /// <summary>Добавить зависимости Application в DI-контейнер.</summary>
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
