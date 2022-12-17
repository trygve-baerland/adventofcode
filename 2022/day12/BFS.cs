using Utils;
namespace Day12;

public static class BFS
{
    public static IEnumerable<Node<int>> AdjacentNodes(this Graph g, Node<int> v, bool noHeight)
    {
        foreach (var dir in Node<int>.Directions)
        {
            var w = v + dir;
            if (g.InGrid(w) &&
                (noHeight || CanTraverse(g, v, w)))
            {
                yield return w;
            }
        }
    }

    public static bool CanTraverse(this Graph g, Node<int> source, Node<int> target)
    {
        char s = MapChar(g.GetHeight(source));
        char t = MapChar(g.GetHeight(target));
        return t - s <= 1;
    }

    public static bool CanDescend(this Graph g, Node<int> source, Node<int> target)
    {
        char s = MapChar(g.GetHeight(source));
        char t = MapChar(g.GetHeight(target));
        return t - s >= -1;
    }

    public static int EdgeWeight(this Graph g, Node<int> source, Node<int> target)
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
}