namespace Mansari.Store.Catalog.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAtUtc { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected Entity() { }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Entity id cannot be empty.");

        Id = id;
        CreatedAtUtc = DateTime.UtcNow;
    }

    protected void MarkAsUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
