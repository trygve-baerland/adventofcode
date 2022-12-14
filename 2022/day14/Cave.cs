namespace Day14;
using Utils;

public enum BlockedEnum
{
    Blocked,
    Free,
    Abyss,
}

public class Cave
{
    private char[][] Array { get; set; }
    private Node TopLeft { get; set; }
    private Node BottomRight { get; set; }
    private HashSet<Node> Blockers { get; } = new();
    private bool WithFloor { get; init; }
    public Cave(IEnumerable<IEnumerable<Node>> rockLines, bool withFloor = false)
    {
        // Get bounding box based on rock lines:
        WithFloor = withFloor;
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
        if (withFloor)
        {
            maxY += 2;
        }
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
        // Draw floor:
        if (withFloor)
        {
            DrawRockLine(new List<Node>() {
                new Node {
                    X = minX, Y = maxY
                },
                BottomRight
            });
        }
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
                Blockers.Add(node);
                if (GetLocalCoords(node) is (int row, int col) x)
                {
                    Array[x.row][x.col] = '#';
                }
            }
            prev = next;
        }
    }

    public void Print()
    {
        foreach (var row in Array)
        {
            Console.WriteLine(new string(row));
        }
        Console.WriteLine($"Bounding box: {TopLeft} <-> {BottomRight}");
    }

    public (int row, int col)? GetLocalCoords(Node node)
    {
        if (!(node >= TopLeft && node <= BottomRight))
        {
            return null;
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
        if (sandPos == start && NotBlocked(sandPos) == BlockedEnum.Blocked)
        {
            return BlockedEnum.Abyss;
        }
        if (blocked == BlockedEnum.Blocked)
        {
            // Draw sand on grid:
            var coords = GetLocalCoords(sandPos);
            if (coords is (int row, int col) x)
            {
                Array[x.row][x.col] = 'o';
            };
            Blockers.Add(sandPos);
        }
        return blocked;
    }

    public BlockedEnum NotBlocked(Node node)
    {
        if (Blockers.Contains(node))
        {
            return BlockedEnum.Blocked;
        }
        else if (node.Y >= BottomRight.Y)
        {
            if (WithFloor) return BlockedEnum.Blocked;
            else return BlockedEnum.Abyss;
        }
        return BlockedEnum.Free;
    }

    public static List<(int x, int y)> DropDirections = new() { (0, 1), (-1, 1), (1, 1) };

}