namespace Utils;

public static class Extension
{
    public static int BFS(this Graph g, Node source, Node target, Func<Node, IEnumerable<Node>> adjacentNodes)
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

    public static int Dijkstra(this Graph g, Node source, Node target, Func<Node, IEnumerable<Node>> adjacentNodes, Func<Node, Node, int> weights)
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