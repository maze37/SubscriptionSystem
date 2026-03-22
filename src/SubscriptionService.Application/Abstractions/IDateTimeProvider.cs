namespace SubscriptionService.Application.Abstractions;

/// <summary>Провайдер текущего времени. Инжектируется как зависимость для тестируемости.</summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}