namespace Utils;
using System.Numerics;

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
public class Graph
{
    public char[][] Array { get; init; } = default!;

    public static Graph FromFile(string path)
    {
        return new Graph
        {
            Array = File.OpenText(path)
                .GetLines()
                .Select(line => line.ToCharArray())
                .ToArray()
        };
    }

    #region Public methods
    public IEnumerable<Node<int>> GetNodes()
    {
        foreach (var idx in Enumerable.Range(0, Array.Length))
        {
            foreach (var idy in Enumerable.Range(0, Array[idx].Length))
            {
                yield return new Node<int> { X = idx, Y = idy };
            }
        }
    }

    public Node<int> IndexOf(char letter)
    {
        var (line, row) = Array.Select((line, ind) => (line, ind))
            .Where(item => item.line.Contains(letter))
            .First();

        var col = System.Array.IndexOf(line, letter);
        return new Node<int> { X = row, Y = col };
    }

    public char GetHeight(Node<int> v)
    {
        try
        {
            return Array[v.X][v.Y];
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to get height for {v}", ex);
        }
    }

    public bool InGrid(Node<int> v)
    {
        return v.X >= 0 && v.X < Array.Length &&
            v.Y >= 0 && v.Y < Array[v.X].Length;
    }
    #endregion Public methods
}