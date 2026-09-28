using System.Reflection;
using Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace SubscriptionService.Application;

/// <summary>
/// Регистрация сервисов слоя Application.
/// Регистрирует обработчики команд и запросов.
/// </summary>
public static class Inject
{
    /// <summary>
    /// Добавить зависимости Application в DI-контейнер.
    /// </summary>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableToAny(
                    typeof(ICommandHandler<,>),
                    typeof(ICommandHandler<>)
                ))
            .AsSelfWithInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableTo(typeof(IQueryHandlerWithResult<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
