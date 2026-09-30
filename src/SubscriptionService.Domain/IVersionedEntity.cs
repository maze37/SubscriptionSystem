namespace SubscriptionService.Domain;

/// <summary>
/// Сущность с версией для оптимистичной блокировки.
/// </summary>
public interface IVersionedEntity
{
    int Version { get; }
    void IncreaseVersion();
}