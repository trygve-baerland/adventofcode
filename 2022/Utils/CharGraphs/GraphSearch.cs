namespace Utils.Graphs;

public static class GraphSearch
{
    public static bool DFSRecursive(
        Node source,
        Func<Node, IEnumerable<Node>> adjacentNodes,
        Func<Node, bool> postVisitor,
        HashSet<Node> visited
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

    public static bool DFSRecursive(
        Node source,
        Func<Node, IEnumerable<Node>> adjacentNodes,
        Func<Node, bool> postVisitor
    )
    {
        return DFSRecursive(source, adjacentNodes, postVisitor, new());
    }

    public static bool DFSIterative(
        Node source,
        Func<Node, IEnumerable<Node>> adjacentNodes,
        Func<Node, bool> postVisitor
    )
    {
        // Initialize stuff:
        var stack = new Stack<(Node node, IEnumerator<Node> neighbours)>();
        var explored = new HashSet<Node>();
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
}