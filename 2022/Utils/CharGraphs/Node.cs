using System.Numerics;

namespace Utils;
public struct Node<T> : IEquatable<Node<T>>
where T : INumber<T>, IBitwiseOperators<T, T, T>, IConvertible
{
    public T X { get; set; }
    public T Y { get; set; }

    public (T x, T y) Point { get => (X, Y); }

    #region Operators
    public static (T x, T y) operator -(Node<T> lhs, Node<T> rhs)
    {
        return (lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static Node<T> operator +(Node<T> lhs, (T x, T y) rhs)
    {
        return new Node<T>
        {
            X = checked(lhs.X + rhs.x),
            Y = checked(lhs.Y + rhs.y)
        };
    }

    public static bool operator ==(Node<T> lhs, Node<T> rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    public bool Equals(Node<T> other)
    {
        return this == other;
    }

    public static bool operator !=(Node<T> lhs, Node<T> rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator <=(Node<T> lhs, Node<T> rhs)
    {
        return lhs.X <= rhs.X && lhs.Y <= rhs.Y;
    }

    public static bool operator >=(Node<T> lhs, Node<T> rhs)
    {
        return lhs.X >= rhs.X && lhs.Y >= rhs.Y;
    }

    #endregion Operators
    #region Misc
    public static IEnumerable<(int x, int y)> Directions { get; } = new List<(int x, int y)> { (-1, 0), (1, 0), (0, -1), (0, 1) };

    public override bool Equals(object? obj)
    {
        if (obj is not Node<T>) return false;
        Node<T> p = (Node<T>)obj;
        return this == p;
    }
    public override int GetHashCode()
    {
        return Convert.ToInt32(X ^ Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public IEnumerable<Node<T>> NodesTo(Node<T> target, bool includeEnd)
    {
        var dx = target - this;
        // Concatenate to one step at a time:
        dx = (
            (T)Convert.ChangeType(Math.Sign(Convert.ToDecimal(dx.x)), typeof(T)),
            (T)Convert.ChangeType(Math.Sign(Convert.ToDecimal(dx.y)), typeof(T))
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