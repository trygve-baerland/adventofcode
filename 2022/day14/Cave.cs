namespace Day14;
using Utils;

public class AbyssException : Exception
{
    public AbyssException(string message)
        : base(message) { }
}

public enum BlockedEnum
{
    Blocked,
    Free,
    Abyss
}

public class Cave
{
    private char[][] Array { get; set; }
    private Node TopLeft { get; set; }
    private Node BottomRight { get; set; }
    public Cave(IEnumerable<IEnumerable<Node>> rockLines)
    {
        // Get bounding box based on rock lines:
        int minX = rockLines.Select(
            rockLine => rockLine.Select(node => node.X).Min()
        ).Min();
        int maxX = rockLines.Select(
            rockLine => rockLine.Select(node => node.X).Max()
        ).Max();
        int minY = rockLines.Select(
            rockLine => rockLine.Select(node => node.Y).Min()
        ).Min();
        minY = Math.Min(0, minY);
        int maxY = rockLines.Select(
            rockLine => rockLine.Select(node => node.Y).Max()
        ).Max();
        TopLeft = new Node { X = minX, Y = minY };
        BottomRight = new Node { X = maxX, Y = maxY };
        // Initialize array:
        Array = Enumerable.Range(0, maxY - minY + 1)
            .Select(
                _ => Enumerable.Range(0, maxX - minX + 1)
                    .Select(_ => '.').ToArray()
            )
            .ToArray();
        // Draw in rocks:
        rockLines.ForEach(rockLine => { DrawRockLine(rockLine); });
    }
    public void DrawRockLine(IEnumerable<Node> rockLine)
    {
        var enumerator = rockLine.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return;
        }
        var prev = enumerator.Current;
        Node next;
        while (enumerator.MoveNext())
        {
            next = enumerator.Current;
            foreach (var node in prev.NodesTo(next, true))
            {
                var (row, col) = GetLocalCoords(node);
                Array[row][col] = '#';
            }
            prev = next;
        }
    }

    public void Print()
    {
        Console.WriteLine($"Bounding box: {TopLeft} <-> {BottomRight}");
        foreach (var row in Array)
        {
            Console.WriteLine(row);
        }
    }

    public (int row, int col) GetLocalCoords(Node node)
    {
        if (!(node >= TopLeft && node <= BottomRight))
        {
            throw new AbyssException($"Node {node} is not in cave");
        }
        return (node.Y - TopLeft.Y, node.X - TopLeft.X);
    }

    public BlockedEnum DropSand(Node start)
    {
        var sandPos = start;
        BlockedEnum blocked = BlockedEnum.Free;
        while (blocked == BlockedEnum.Free)
        {
            (sandPos, blocked) = DropDirections // Go through each direction:
                .Select(dir => sandPos + dir)
                .Select(pos => (pos, NotBlocked(pos))) // update new position
                .FirstOrDefault(
                    item => item.Item2 != BlockedEnum.Blocked,
                    (sandPos, BlockedEnum.Blocked));
        }
        if (blocked == BlockedEnum.Blocked)
        {
            // Draw sand on grid:
            var (row, col) = GetLocalCoords(sandPos);
            Array[row][col] = 'o';
        }
        return blocked;
    }

    public BlockedEnum NotBlocked(Node node)
    {
        try
        {
            return NotBlocked(GetLocalCoords(node));
        }
        catch (AbyssException)
        {
            return BlockedEnum.Abyss;
        }
    }

    public BlockedEnum NotBlocked((int row, int col) coord)
    {
        return Array[coord.row][coord.col] switch
        {
            '#' => BlockedEnum.Blocked,
            'o' => BlockedEnum.Blocked,
            _ => BlockedEnum.Free
        };
    }

    public static List<(int x, int y)> DropDirections = new() { (0, 1), (-1, 1), (1, 1) };

}