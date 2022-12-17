using Utils;

namespace Day15;

public class Day15Helpers
{
    public static int L1((int x, int y) tv)
    {
        return Math.Abs(tv.x) + Math.Abs(tv.y);
    }

    public static int L1Dist(Node<int> lhs, Node<int> rhs)
    {
        return L1(rhs - lhs);
    }

    public static bool CanNotHaveBeacon(Node<int> node, IEnumerable<(Node<int> sensor, int dist)> sensors)
    {
        return sensors
            .Where(item => L1Dist(node, item.sensor) <= item.dist)
            .Any();
    }

    public static IEnumerable<Node<int>> FeasibleNodes(IEnumerable<(Node<int> sensor, int dist)> pairs, HashSet<Node<int>> beacons)
    {
        int minY = pairs.Select(pair => pair.sensor.Y - pair.dist).Min();
        int maxY = pairs.Select(pair => pair.sensor.Y + pair.dist).Max();
        foreach (var y in Enumerable.Range(minY, maxY - minY + 1))
        {
            foreach (var node in FeasibleNodes(pairs, y, beacons))
            {
                yield return node;
            }
        }
    }
    public static IEnumerable<Node<int>> FeasibleNodes(IEnumerable<(Node<int> sensor, int dist)> pairs, int y, HashSet<Node<int>> beacons)
    {

        foreach (var (sensor, dist) in pairs)
        {
            var dy = Math.Abs(sensor.Y - y);
            if (dy <= dist)
            {
                var D = dist - dy;
                for (int x = sensor.X - D; x <= sensor.X + D; x++)
                {
                    var candidate = new Node<int> { X = x, Y = y };
                    if (!beacons.Contains(candidate))
                        yield return candidate;
                }
            }
        }
    }

    public static IEnumerable<Node<int>> SearchSignal((int min, int max) box, IEnumerable<(Node<int> sensor, int dist)> pairs, HashSet<Node<int>> beacons)
    {
        // Go though each sensor
        HashSet<Node<int>> visited = new();
        foreach (var (sensor, dist) in pairs)
        {
            foreach (var node in EquiDistantNodes(sensor, dist + 1))
            {
                if (node.X >= box.min && node.Y >= box.min && node.X <= box.max && node.Y <= box.max && !CanNotHaveBeacon(node, pairs))
                {
                    if (!visited.Contains(node))
                    {
                        visited.Add(node);
                        yield return node;
                    }
                }
            }
        }
    }

    public static long TuningFrequency(Node<int> node)
    {
        return 4000000 * ((long)node.X) + node.Y;
    }

    public static IEnumerable<Node<int>> EquiDistantNodes(Node<int> center, int dist)
    {
        var node = new Node<int>
        {
            X = center.X - dist,
            Y = center.Y
        };
        foreach (var dir in Directions)
        {
            foreach (var _ in Enumerable.Range(0, dist))
            {
                yield return node;
                node += dir;
            }
        }
    }

    public static readonly List<(int dx, int dy)> Directions = new() { (1, -1), (1, 1), (-1, 1), (-1, -1) };
}