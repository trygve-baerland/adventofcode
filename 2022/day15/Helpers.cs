using Utils;

namespace Day15;


public class Day15Helpers
{
    public static int L1((int x, int y) tv)
    {
        return Math.Abs(tv.x) + Math.Abs(tv.y);
    }

    public static int L1Dist(Node lhs, Node rhs)
    {
        return L1(rhs - lhs);
    }

    public static bool CanNotHaveBeacon(Node node, IEnumerable<(Node sensor, int dist)> sensors)
    {
        return sensors
            .Where(item => L1Dist(node, item.sensor) <= item.dist)
            .Any();
    }

    public static IEnumerable<Node> FeasibleNodes(IEnumerable<(Node sensor, int dist)> pairs, HashSet<Node> beacons)
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
    public static IEnumerable<Node> FeasibleNodes(IEnumerable<(Node sensor, int dist)> pairs, int y, HashSet<Node> beacons)
    {

        foreach (var (sensor, dist) in pairs)
        {
            var dy = Math.Abs(sensor.Y - y);
            if (dy <= dist)
            {
                var D = dist - dy;
                for (int x = sensor.X - D; x <= sensor.X + D; x++)
                {
                    var candidate = new Node { X = x, Y = y };
                    if (!beacons.Contains(candidate))
                        yield return candidate;
                }
            }
        }
    }

    public static IEnumerable<Node> SearchSignal((int min, int max) box, IEnumerable<(Node sensor, int dist)> pairs, HashSet<Node> beacons)
    {
        // Go though each sensor
        HashSet<Node> visited = new();
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

    public static int TuningFrequency(Node node)
    {
        return 4000000 * node.X + node.Y;
    }

    public static IEnumerable<Node> EquiDistantNodes(Node center, int dist)
    {
        var corners = new Queue<Node>(new List<Node> {
            new Node{
                X = center.X - dist,
                Y = center.Y
            },
            new Node {
                X = center.X,
                Y = center.Y - dist
            },
            new Node {
                X = center.X + dist,
                Y = center.Y
            },
            new Node {
                X = center.X,
                Y = center.Y + dist
            }
        }
        );
        foreach (var d in Enumerable.Range(0, dist))
        {
            foreach (var id in Enumerable.Range(0, 4))
            {
                var corner = corners.Dequeue();
                yield return corner;
                corners.Enqueue(corner + Directions[id]);
            }
        }
    }

    public static readonly List<(int dx, int dy)> Directions = new() { (1, -1), (1, 1), (-1, 1), (-1, -1) };
}