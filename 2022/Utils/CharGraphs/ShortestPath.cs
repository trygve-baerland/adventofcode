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

    public static int CheckedSum(int a, int b, int defaultValue = int.MaxValue)
    {
        try
        {
            return checked(a + b);
        }
        catch (OverflowException)
        {
            return defaultValue;
        }
    }

    public static ShortestPathMapping<TNode> FloydWarshall<TNode>(
        IEnumerable<TNode> nodes,
        Func<TNode, IEnumerable<TNode>> adjacentNodes)
    where TNode : notnull
    {
        var result = new ShortestPathMapping<TNode>(nodes, adjacentNodes);
        result.FloydWarshall();
        return result;
    }
}

public class IndexMapping<TNode>
where TNode : notnull
{
    public Dictionary<TNode, int> Mapping { get; private set; }
    public IndexMapping(IEnumerable<TNode> nodes)
    {
        Mapping = nodes.Select((node, i) => (node, i))
            .ToDictionary(
                item => item.node,
                item => item.i
            );
    }
    public int Get(TNode node) => Mapping[node];
    public bool TryGet(TNode node, out int value) => Mapping.TryGetValue(node, out value);
    public int NumNodes { get => Mapping.Count; }

    public TNode FromIndex(int i)
    {
        if (i < 0 || i >= NumNodes)
        {
            throw new Exception($"Invalid index {i}");
        }
        return Mapping.Where(item => item.Value == i)
            .Select(item => item.Key)
            .First();
    }
}

public class ShortestPathMapping<TNode>
where TNode : notnull
{
    public IndexMapping<TNode> Index { get; private set; }
    public int[,] Distances { get; private set; }

    public int Size { get => Index.NumNodes; }

    public ShortestPathMapping(IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>> adjacentNodes)
    {
        Index = new IndexMapping<TNode>(nodes);
        Distances = new int[Size, Size];
        // Initialize with int.MaxValue:
        foreach (var i in Enumerable.Range(0, Size))
        {
            foreach (var j in Enumerable.Range(0, Size))
            {
                Distances[i, j] = int.MaxValue;
            }
            Distances[i, i] = 0;
        }
        // Add edges:
        foreach (var u in nodes)
        {
            int i = Index.Get(u);
            foreach (var v in adjacentNodes(u))
            {
                int j = Index.Get(v);
                Distances[i, j] = 1;
            }
        }
    }

    public void FloydWarshall()
    {
        foreach (var k in Enumerable.Range(0, Size))
        {
            foreach (var i in Enumerable.Range(0, Size))
            {
                foreach (var j in Enumerable.Range(0, Size))
                {
                    var candidate = ShortestPath.CheckedSum(Distances[i, k], Distances[k, j]);
                    if (candidate < Distances[i, j])
                    {
                        Distances[i, j] = candidate;
                    }
                }
            }
        }
    }

    public int GetDistance(TNode source, TNode target)
    {
        return Distances[
            Index.Get(source),
            Index.Get(target)
        ];
    }

    public IEnumerable<(TNode node, int dist)> GetFrom<TKey>(TNode node, Func<TNode, TKey> selector)
    {
        var i = Index.Get(node);
        return Enumerable.Range(0, Size)
            .Where(j => j != i)
            .Select(j => (Index.FromIndex(j), Distances[i, j]))
            .OrderBy(item => selector(item.Item1));
    }
}