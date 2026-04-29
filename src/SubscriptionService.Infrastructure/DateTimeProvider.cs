using SubscriptionService.Application.Abstractions;

namespace SubscriptionService.Infrastructure;

/// <summary>Реальная реализация провайдера времени.</summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>Текущее UTC-время сервера.</summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
