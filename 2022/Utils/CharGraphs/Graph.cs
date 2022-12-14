namespace Utils;

public struct Node
{
    public int X { get; set; }
    public int Y { get; set; }

    public (int x, int y) Point { get => (X, Y); }

    #region Operators
    public static (int x, int y) operator -(Node lhs, Node rhs)
    {
        return (lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static Node operator +(Node lhs, (int x, int y) rhs)
    {
        return new Node
        {
            X = lhs.X + rhs.x,
            Y = lhs.Y + rhs.y
        };
    }

    public static bool operator ==(Node lhs, Node rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    public static bool operator !=(Node lhs, Node rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator <=(Node lhs, Node rhs)
    {
        return lhs.X <= rhs.X && lhs.Y <= rhs.Y;
    }

    public static bool operator >=(Node lhs, Node rhs)
    {
        return lhs.X >= rhs.X && lhs.Y >= rhs.Y;
    }

    #endregion Operators
    #region Misc
    public static IEnumerable<(int x, int y)> Directions { get; } = new List<(int x, int y)> { (-1, 0), (1, 0), (0, -1), (0, 1) };

    public override bool Equals(object? obj)
    {
        if (obj is not Node) return false;
        Node p = (Node)obj;
        return this == p;
    }
    public override int GetHashCode()
    {
        return X ^ Y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public IEnumerable<Node> NodesTo(Node target, bool includeEnd)
    {
        var dx = target - this;
        // Concatenate to one step at a time:
        dx = (Math.Sign(dx.x), Math.Sign(dx.y));
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
    public IEnumerable<Node> GetNodes()
    {
        foreach (var idx in Enumerable.Range(0, Array.Length))
        {
            foreach (var idy in Enumerable.Range(0, Array[idx].Length))
            {
                yield return new Node { X = idx, Y = idy };
            }
        }
    }

    public Node IndexOf(char letter)
    {
        var (line, row) = Array.Select((line, ind) => (line, ind))
            .Where(item => item.line.Contains(letter))
            .First();

        var col = System.Array.IndexOf(line, letter);
        return new Node { X = row, Y = col };
    }

    public char GetHeight(Node v)
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

    public bool InGrid(Node v)
    {
        return v.X >= 0 && v.X < Array.Length &&
            v.Y >= 0 && v.Y < Array[v.X].Length;
    }
    #endregion Public methods
}