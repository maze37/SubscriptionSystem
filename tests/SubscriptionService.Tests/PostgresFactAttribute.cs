using Xunit;

namespace SubscriptionService.Tests;

public sealed class PostgresFactAttribute : FactAttribute
{
    public const string ConnectionStringVariable = "SUBSCRIPTION_TEST_CONNECTION";

    public PostgresFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionStringVariable)))
            Skip = $"Задайте {ConnectionStringVariable} для запуска PostgreSQL integration-тестов.";
    }
}
