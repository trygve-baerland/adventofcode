using System.Diagnostics.CodeAnalysis;
using Utils;

namespace Day18;

public class LavaClump
{
    public HashSet<Node3D<int>> Points = new();

    public int FreeSides { get; private set; } = 0;

    public void AddPoint(Node3D<int> point)
    {
        // First wee add 6 new sides:
        FreeSides += 6;
        // Then we subtract for every blocked side:
        foreach (var dir in Node3D<int>.Directions)
        {
            if (Points.Contains(point + dir))
            {
                FreeSides -= 2;
            }
        }
        // Finally, we add  the point to the clump:
        Points.Add(point);
    }

    public IEnumerable<SurfaceEdge> GetFreeSides()
    {
        // Go through each point
        foreach (var point in Points)
        {
            foreach (var dir in Node3D<int>.Directions)
            {
                if (!Points.Contains(point + dir))
                {
                    yield return new SurfaceEdge
                    {
                        Point = point,
                        Direction = dir
                    };
                }
            }
        }
    }

    public void FillVoidFrom(Node3D<int> source)
    {
        if (Points.Contains(source)) return;
        var toVisit = new Queue<Node3D<int>>();
        toVisit.Enqueue(source);
        var explored = new HashSet<Node3D<int>>();
        while (toVisit.TryDequeue(out var node))
        {
            if (!explored.Contains(node))
            {
                explored.Add(node);
                AddPoint(node);
                // Get adjacent nodes:
                foreach (var dir in Node3D<int>.Directions)
                {
                    var candidate = node + dir;
                    if (!Points.Contains(candidate))
                    {
                        toVisit.Enqueue(candidate);
                    }
                }
            }
        }
    }

    public IEnumerable<Node3D<int>> GetAdjacentPoints(Node3D<int> source)
    {
        foreach (var dir in Node3D<int>.Directions)
        {
            var candidate = source + dir;
            if (Points.Contains(candidate))
            {
                yield return candidate;
            }
        }
    }
    public IEnumerable<Node3D<int>> GetAdjacentVoids(Node3D<int> source)
    {
        foreach (var dir in Node3D<int>.Directions)
        {
            var candidate = source + dir;
            if (!Points.Contains(candidate))
            {
                yield return candidate;
            }
        }
    }
    public List<(int min, int max)> GetBoundingBox()
    {
        return new List<(int min, int max)>
        {
            (Points.Select(point => point.X).Min() - 1, Points.Select(point => point.X).Max() + 1),
            (Points.Select(point => point.Y).Min() - 1, Points.Select(point => point.Y).Max() + 1),
            (Points.Select(point => point.Z).Min() - 1, Points.Select(point => point.Z).Max() + 1)
        };
    }
}

public class OutsideLavaClump
{
    public LavaClump Clump { get; private set; }
    public List<(int min, int max)> BoundingBox { get; }
    public HashSet<Node3D<int>> OutSidePoints { get; } = new();

    public int FreeSides { get; private set; } = 0;

    public OutsideLavaClump(LavaClump clump)
    {
        Clump = clump;
        BoundingBox = Clump.GetBoundingBox();
    }

    public void FillOutside()
    {
        var toVisit = new Queue<Node3D<int>>();
        toVisit.Enqueue(new Node3D<int>
        {
            X = BoundingBox[0].min,
            Y = BoundingBox[1].min,
            Z = BoundingBox[2].min
        });
        while (toVisit.TryDequeue(out var node))
        {
            if (!OutSidePoints.Contains(node))
            {
                AddPoint(node);
                foreach (var w in GetAdjacentNodes(node))
                {
                    toVisit.Enqueue(w);
                }
            }
        }

    }

    public void AddPoint(Node3D<int> point)
    {
        OutSidePoints.Add(point);
        FreeSides += 6;
        foreach (var dir in Node3D<int>.Directions)
        {
            var candidate = point + dir;
            if (OutSidePoints.Contains(candidate))
            {
                FreeSides -= 2;
            }
        }
        // Make sure we handle the edges properly:
        if (point.X == BoundingBox[0].min || point.X == BoundingBox[0].max)
        {
            FreeSides -= 1;
        }
        if (point.Y == BoundingBox[1].min || point.Y == BoundingBox[1].max)
        {
            FreeSides -= 1;
        }
        if (point.Z == BoundingBox[2].min || point.Z == BoundingBox[2].max)
        {
            FreeSides -= 1;
        }
    }

    public IEnumerable<Node3D<int>> GetAdjacentNodes(Node3D<int> source)
    {
        foreach (var dir in Node3D<int>.Directions)
        {
            var candidate = source + dir;
            if (InBox(candidate) && !Clump.Points.Contains(candidate))
            {
                yield return candidate;
            }
        }
    }

    public bool InBox(Node3D<int> point)
    {
        return point.X >= BoundingBox[0].min && point.X <= BoundingBox[0].max &&
            point.Y >= BoundingBox[1].min && point.Y <= BoundingBox[1].max &&
            point.Z >= BoundingBox[2].min && point.Z <= BoundingBox[2].max;
    }
}

public struct SurfaceEdge : IEqualityComparer<SurfaceEdge>
{
    public Node3D<int> Point { get; set; }
    public (int x, int y, int z) Direction { get; set; }

    public override string ToString()
    {
        return $"P: {Point}, D: {Direction}";
    }

    public static (int x, int y, int z) ReverseDirection((int x, int y, int z) dir)
    {
        return (-dir.x, -dir.y, -dir.z);
    }

    public static IEnumerable<SurfaceEdge> GetAdjacentSides(SurfaceEdge edge)
    {
        // Get index of direction (x, y, z).
        var arrity = Node3D<int>.Directions.IndexOf(edge.Direction) / 2;

        // Only go through directions with different arrity:
        foreach (var dir in Node3D<int>.Directions
            .Select((xdir, i) => (xdir, i / 2))
            .Where(item => item.Item2 != arrity)
            .Select(item => item.xdir))
        {
            // First the edges on the same box:
            yield return new SurfaceEdge
            {
                Point = edge.Point,
                Direction = dir
            };
            // Next, the boxes next to our current one:
            yield return new SurfaceEdge
            {
                Point = edge.Point + dir,
                Direction = edge.Direction
            };
            // Finally, those that are hard to describe:
            yield return new SurfaceEdge
            {
                Point = edge.Point + edge.Direction + dir,
                Direction = ReverseDirection(dir)
            };
        }
    }

    public bool Equals(SurfaceEdge x, SurfaceEdge y)
    {
        return x.Point == y.Point && x.Direction == y.Direction;
    }

    public int GetHashCode([DisallowNull] SurfaceEdge obj)
    {
        return Point.GetHashCode() ^ Direction.GetHashCode();
    }

}