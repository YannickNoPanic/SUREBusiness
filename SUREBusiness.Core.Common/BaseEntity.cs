namespace SUREBusiness.Core.Common;

public class BaseEntity : IEquatable<BaseEntity>
{
    public int Id { get; set; }

    public bool Equals(BaseEntity? other)
    {
        if (GetType() == other?.GetType())
        {
            return Id.Equals(other?.Id);
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BaseEntity other)
        {
            return Equals(other);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType().GetHashCode());
    }
}
