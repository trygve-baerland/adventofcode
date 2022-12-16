namespace Utils.Graphs;

public static class ShortestPath
{
    public static int BFS<TNode>(TNode source, TNode target, Func<TNode, IEnumerable<TNode>> adjacentNodes)
    where TNode : IEquatable<TNode>
    {
        var toVisit = new Queue<(TNode index, int dist)>();
        toVisit.Enqueue((source, 0));
        var explored = new HashSet<TNode>();
        while (toVisit.TryDequeue(out var item))
        {
            var (v, dist) = item;
            if (v.Equals(target)) return dist;
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

    public static int BFS<TNode>(IEnumerable<TNode> sources, TNode target, Func<TNode, IEnumerable<TNode>> adjacentNodes)
    where TNode : IEquatable<TNode>
    {
        var toVisit = new Queue<(TNode index, int dist)>();
        sources.ForEach(s => toVisit.Enqueue((s, 0)));
        var explored = new HashSet<TNode>();
        while (toVisit.TryDequeue(out var item))
        {
            var (v, dist) = item;
            if (v.Equals(target)) return dist;
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

    public static int BFS<TNode>(TNode source, Func<TNode, bool> target, Func<TNode, IEnumerable<TNode>> adjacentNodes)
    {
        var toVisit = new Queue<(TNode index, int dist)>();
        toVisit.Enqueue((source, 0));
        var explored = new HashSet<TNode>();
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

    public static int Dijkstra<TNode>(TNode source, TNode target, Func<TNode, IEnumerable<TNode>> adjacentNodes, Func<TNode, TNode, int> weights)
    where TNode : IEquatable<TNode>
    {
        var toVisit = new PriorityQueue<TNode, int>();
        toVisit.Enqueue(source, 0);
        var explored = new HashSet<TNode>();

        while (toVisit.TryDequeue(out var v, out var dist))
        {
            if (v.Equals(target)) return dist;
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