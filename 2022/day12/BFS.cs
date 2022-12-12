using Utils;
namespace Day12;

public static class BFS
{
    public static IEnumerable<Node> AdjacentNodes(Graph g, Node v, bool noHeight)
    {
        foreach (var dir in Node.Directions)
        {
            var w = v + dir;
            if (g.InGrid(w) &&
                (noHeight || CanTraverse(g, v, w)))
            {
                yield return w;
            }
        }
    }

    public static bool CanTraverse(Graph g, Node source, Node target)
    {
        char s = MapChar(g.GetHeight(source));
        char t = MapChar(g.GetHeight(target));
        return t - s <= 1;
    }

    public static int EdgeWeight(Graph g, Node source, Node target)
    {
        char s = MapChar(g.GetHeight(source));
        char t = MapChar(g.GetHeight(target));
        if (t - s > 1)
        {
            return int.MaxValue;
        }
        else
        {
            return 1;
        }
    }

    public static char MapChar(char c)
    {
        if (c == 'S') return 'a';
        if (c == 'E') return 'z';
        return c;
    }

    public static int GetCandidateDistance(int oldDistance, int edgeWeight)
    {
        try
        {
            return checked(oldDistance + edgeWeight);
        }
        catch (OverflowException)
        {
            return int.MaxValue;
        }
    }

    public static IEnumerable<(int row, int col)> GetIndices(char[][] arr)
    {
        foreach (var ix in Enumerable.Range(0, arr.Length))
        {
            foreach (var iy in Enumerable.Range(0, arr[ix].Length))
            {
                yield return (ix, iy);
            }
        }
    }

}