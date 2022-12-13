namespace Utils.Graphs;

public static class ShortestPath
{
    public static int BFS(Node source, Node target, Func<Node, IEnumerable<Node>> adjacentNodes)
    {
        var toVisit = new Queue<(Node index, int dist)>();
        toVisit.Enqueue((source, 0));
        var explored = new HashSet<Node>();
        while (toVisit.TryDequeue(out var item))
        {
            var (v, dist) = item;
            if (v == target) return dist;
            if (!explored.Contains(v))
            {
                explored.Add(v);
                foreach (var w in adjacentNodes(v))
                {
                    toVisit.Enqueue((w, dist + 1));
                }
            }
        }
        return int.MaxValue;
    }

    public static int BFS(IEnumerable<Node> sources, Node target, Func<Node, IEnumerable<Node>> adjacentNodes)
    {
        var toVisit = new Queue<(Node index, int dist)>();
        sources.ForEach(s => toVisit.Enqueue((s, 0)));
        var explored = new HashSet<Node>();
        while (toVisit.TryDequeue(out var item))
        {
            var (v, dist) = item;
            if (v == target) return dist;
            if (!explored.Contains(v))
            {
                explored.Add(v);
                foreach (var w in adjacentNodes(v))
                {
                    toVisit.Enqueue((w, dist + 1));
                }
            }
        }
        return int.MaxValue;
    }

    public static int BFS(Node source, Func<Node, bool> target, Func<Node, IEnumerable<Node>> adjacentNodes)
    {
        var toVisit = new Queue<(Node index, int dist)>();
        toVisit.Enqueue((source, 0));
        var explored = new HashSet<Node>();
        while (toVisit.TryDequeue(out var item))
        {
            var (v, dist) = item;
            if (target(v)) return dist;
            if (!explored.Contains(v))
            {
                explored.Add(v);
                foreach (var w in adjacentNodes(v))
                {
                    toVisit.Enqueue((w, dist + 1));
                }
            }
        }
        return int.MaxValue;
    }

    public static int Dijkstra(Node source, Node target, Func<Node, IEnumerable<Node>> adjacentNodes, Func<Node, Node, int> weights)
    {
        var toVisit = new PriorityQueue<Node, int>();
        toVisit.Enqueue(source, 0);
        var explored = new HashSet<Node>();

        while (toVisit.TryDequeue(out var v, out var dist))
        {
            if (v == target) return dist;
            if (!explored.Contains(v))
            {
                explored.Add(v);
                foreach (var w in adjacentNodes(v))
                {
                    toVisit.Enqueue(w, CheckedSum(dist, weights(v, w)));
                }
            }
        }
        return int.MaxValue;
    }

    private static int CheckedSum(int a, int b)
    {
        try
        {
            return checked(a + b);
        }
        catch (OverflowException)
        {
            return int.MaxValue;
        }
    }
}