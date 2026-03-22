using SubscriptionService.Application.Abstractions;

namespace SubscriptionService.Infrastructure;

/// <summary>Реальная реализация провайдера времени.</summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}