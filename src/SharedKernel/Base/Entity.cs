namespace SharedKernel.Base;

/// <summary>
/// Базовый класс сущности с идентификатором типа Guid.
/// Равенство определяется по типу и Id — два объекта равны если они одного типа и имеют одинаковый Id.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; private set; }

    protected Entity() {}

    protected Entity(Guid id) => Id = id;

    public override bool Equals(object? obj)
    {
        if(obj is not Entity other)
            return false;

        if (!ReferenceEquals(this, other) || !EqualityComparer<Guid>.Default.Equals(Id, other.Id))
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id.ToString()).GetHashCode(StringComparison.Ordinal);
    }
    
    
    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b) => !(a == b);
}

/// <summary>
/// Базовый класс сущности с идентификатором произвольного типа.
/// Используется когда Id не является Guid.
/// </summary>
public abstract class Entity<TId>
    where TId:notnull
{
    public required TId Id { get; init; }

    protected Entity() {}

    protected Entity(TId id) => Id = id;

    public override bool Equals(object? obj)
    {
        if(obj is not Entity<TId> other)
            return false;

        if (!ReferenceEquals(this, other) || !EqualityComparer<TId>.Default.Equals(Id, other.Id))
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id.ToString()).GetHashCode(StringComparison.Ordinal);
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b) => !(a == b);
}