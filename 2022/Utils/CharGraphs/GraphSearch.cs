namespace Utils.Graphs;

public static class GraphSearch
{
    public static bool DFSRecursive<TNode>(
        TNode source,
        Func<TNode, IEnumerable<TNode>> adjacentNodes,
        Func<TNode, bool> postVisitor,
        HashSet<TNode> visited
    )
    {
        if (!visited.Contains(source))
        {
            visited.Add(source);
            foreach (var node in adjacentNodes(source))
            {
                if (DFSRecursive(node, adjacentNodes, postVisitor, visited)) return true;
            }
            return postVisitor(source);
        }
        return false;
    }

    public static bool DFSRecursive<TNode>(
        TNode source,
        Func<TNode, IEnumerable<TNode>> adjacentNodes,
        Func<TNode, bool> postVisitor
    )
    {
        return DFSRecursive(source, adjacentNodes, postVisitor, new());
    }

    public static bool DFSIterative<TNode>(
        TNode source,
        Func<TNode, IEnumerable<TNode>> adjacentNodes,
        Func<TNode, bool> postVisitor
    )
    {
        // Initialize stuff:
        var stack = new Stack<(TNode node, IEnumerator<TNode> neighbours)>();
        var explored = new HashSet<TNode>();
        stack.Push((source, adjacentNodes(source).GetEnumerator()));
        while (stack.Count > 0)
        {
            if (stack.Peek().neighbours.MoveNext())
            {
                var w = stack.Peek().neighbours.Current;
                if (!explored.Contains(w))
                {
                    explored.Add(w);
                    stack.Push((w, adjacentNodes(w).GetEnumerator()));
                }
            }
            else
            {
                if (postVisitor(stack.Pop().node)) return true;
            }
        }
        return false;
    }

    public static void BFSIterative<TNode>(
        List<TNode> nodes,
        Func<TNode, IEnumerable<TNode>> adjacentNodes,
        Action<TNode> visitor,
        Action componentVisitor
    )
    {
        var toVisit = new Queue<TNode>();
        var explored = new HashSet<TNode>();
        foreach (var node in nodes)
        {
            //Console.WriteLine($"Starting on node {node}");
            // See if node has already been explored, i.e. we don't care:
            if (!explored.Contains(node))
            {
                Console.WriteLine($"Starting on new component {node}");
                explored.Add(node);
                // We do a BFS search on this node:
                toVisit.Enqueue(node);
                while (toVisit.TryDequeue(out var item))
                {
                    visitor(item);
                    // Go through each adjacent node:
                    foreach (var neighbour in adjacentNodes(item))
                    {
                        if (!explored.Contains(neighbour))
                        {
                            explored.Add(neighbour);
                            toVisit.Enqueue(neighbour);
                        }
                    }
                }
                componentVisitor();
            }

        }
    }
}