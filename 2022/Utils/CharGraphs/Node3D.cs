using System.Numerics;

namespace Utils;
public struct Node3D<T> : IEquatable<Node3D<T>>
where T : INumber<T>, IBitwiseOperators<T, T, T>, IConvertible
{
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }

    public (T x, T y, T z) Point { get => (X, Y, Z); }

    #region Operators
    public static (T x, T y, T z) operator -(Node3D<T> lhs, Node3D<T> rhs)
    {
        return (lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
    }

    public static Node3D<T> operator +(Node3D<T> lhs, (T x, T y, T z) rhs)
    {
        return new Node3D<T>
        {
            X = checked(lhs.X + rhs.x),
            Y = checked(lhs.Y + rhs.y),
            Z = checked(lhs.Z + rhs.z)
        };
    }

    public static bool operator ==(Node3D<T> lhs, Node3D<T> rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
    }

    public bool Equals(Node3D<T> other)
    {
        return this == other;
    }

    public static bool operator !=(Node3D<T> lhs, Node3D<T> rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator <=(Node3D<T> lhs, Node3D<T> rhs)
    {
        return lhs.X <= rhs.X && lhs.Y <= rhs.Y && lhs.Z == rhs.Z;
    }

    public static bool operator >=(Node3D<T> lhs, Node3D<T> rhs)
    {
        return lhs.X >= rhs.X && lhs.Y >= rhs.Y && lhs.Z >= rhs.Z;
    }

    #endregion Operators
    #region Misc
    public static IEnumerable<(int x, int y, int z)> Directions { get; } = new List<(int x, int y, int z)>
        { (-1, 0, 0), (1, 0, 0),
          (0, -1, 0), (0, 1, 0),
          (0, 0, -1), (0, 0, 1)
        };

    public override bool Equals(object? obj)
    {
        if (obj is not Node3D<T>) return false;
        Node3D<T> p = (Node3D<T>)obj;
        return this == p;
    }
    public override int GetHashCode()
    {
        return Convert.ToInt32(X ^ Y ^ Z);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public IEnumerable<Node3D<T>> NodesTo(Node3D<T> target, bool includeEnd)
    {
        var dx = target - this;
        // Concatenate to one step at a time:
        dx = (
            (T)Convert.ChangeType(Math.Sign(Convert.ToDecimal(dx.x)), typeof(T)),
            (T)Convert.ChangeType(Math.Sign(Convert.ToDecimal(dx.y)), typeof(T)),
            (T)Convert.ChangeType(Math.Sign(Convert.ToDecimal(dx.z)), typeof(T))
        );
        var current = this;
        while (current != target)
        {
            yield return current;
            current += dx;
        }
        if (includeEnd)
        {
            yield return current;
        }

    }
    #endregion Misc
}