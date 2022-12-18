namespace Utils;


public class Graph
{
    public char[][] Array { get; init; } = default!;

    public static Graph FromFile(string path)
    {
        return new Graph
        {
            Array = File.OpenText(path)
                .GetLines()
                .Select(line => line.ToCharArray())
                .ToArray()
        };
    }

    #region Public methods
    public IEnumerable<Node<int>> GetNodes()
    {
        foreach (var idx in Enumerable.Range(0, Array.Length))
        {
            foreach (var idy in Enumerable.Range(0, Array[idx].Length))
            {
                yield return new Node<int> { X = idx, Y = idy };
            }
        }
    }

    public Node<int> IndexOf(char letter)
    {
        var (line, row) = Array.Select((line, ind) => (line, ind))
            .Where(item => item.line.Contains(letter))
            .First();

        var col = System.Array.IndexOf(line, letter);
        return new Node<int> { X = row, Y = col };
    }

    public char GetHeight(Node<int> v)
    {
        try
        {
            return Array[v.X][v.Y];
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to get height for {v}", ex);
        }
    }

    public bool InGrid(Node<int> v)
    {
        return v.X >= 0 && v.X < Array.Length &&
            v.Y >= 0 && v.Y < Array[v.X].Length;
    }
    #endregion Public methods
}