using Utils;
// Read input:
var graph = Graph.FromFile("input.txt");

var result1 = graph.GetNodes()
    .Where(v => Helpers.Visible(graph, v))
    .Count();

var result2 = graph.GetNodes()
    .Select(v => Helpers.Scenic(graph, v))
    .Max();

Console.WriteLine($"Part 1: {result1}");
Console.WriteLine($"Part 2: {result2}");
class Helpers
{
    public static IEnumerable<Node<int>> GetAllInDirection(Graph g, Node<int> v, (int x, int y) dir)
    {
        v += dir;
        while (g.InGrid(v))
        {
            yield return v;
            v += dir;
        }
    }

    public static bool VisibleInDirection(Graph g, Node<int> v, (int x, int y) dir)
    {
        return !GetAllInDirection(g, v, dir)
            .Where(w => g.GetHeight(w) >= g.GetHeight(v))
            .Any();
    }
    public static bool Visible(Graph g, Node<int> v)
    {
        return Node<int>.Directions
            .Where(dir => VisibleInDirection(g, v, dir))
            .Any();
    }

    public static int Scenic(Graph g, IEnumerable<Node<int>> sightLine, char height)
    {
        return sightLine.Select((tree, index) => (tree, index))
            .Where(item => g.GetHeight(item.tree) >= height)
            .Select(item => item.index + 1)
            .FirstOrDefault(sightLine.Count());
    }

    public static int Scenic(Graph g, Node<int> v)
    {
        var height = g.GetHeight(v);
        return Node<int>.Directions
            .Select(dir => GetAllInDirection(g, v, dir))
            .Select(sightLine => Scenic(g, sightLine, height))
            .Aggregate((acc, item) => acc * item);
    }
}