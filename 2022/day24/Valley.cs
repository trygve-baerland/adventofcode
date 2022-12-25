using Utils;
namespace Day24;

public enum Direction
{
    R,
    D,
    L,
    U
}
public class Blizzard
{
    public Node<int> Position { get; set; }
    public Direction MovingDirection { get; set; }

    public Blizzard(int x, int y, char c)
    {
        Position = new Node<int> { X = x, Y = y };
        MovingDirection = c switch
        {
            '^' => Direction.U,
            '>' => Direction.R,
            'v' => Direction.D,
            '<' => Direction.L,
            _ => throw new Exception($"Unsupported blizzard direction '{c}'")
        };
    }

    public Node<int> PositionAtTime(int time, int width, int height)
    {
        if (MovingDirection == Direction.L)
        {
            return new Node<int>
            {
                X = Helpers.MathMod(Position.X - time - 1, width),
                Y = Position.Y - 1
            };
        }
        if (MovingDirection == Direction.R)
        {
            return new Node<int>
            {
                X = Helpers.MathMod(Position.X + time - 1, width),
                Y = Position.Y - 1
            };
        }
        if (MovingDirection == Direction.D)
        {
            return new Node<int>
            {
                X = Position.X - 1,
                Y = Helpers.MathMod(Position.Y + time - 1, height)
            };
        }
        if (MovingDirection == Direction.U)
        {
            return new Node<int>
            {
                X = Position.X - 1,
                Y = Helpers.MathMod(Position.Y - time - 1, height)
            };
        }
        throw new Exception($"Unexpected direction {MovingDirection}");
    }


    public char GetChar() => MovingDirection switch
    {
        Direction.U => '^',
        Direction.R => '>',
        Direction.D => 'v',
        Direction.L => '<',
        _ => throw new Exception($"Unsupported direction {MovingDirection}")
    };
}

public class Valley
{
    public int Width { get; init; }
    public int Height { get; init; }
    public int Lcm { get; private set; }
    public List<Blizzard> Blizzards { get; init; }
    public bool[,,] BlizzardPos { get; private set; }
    public Valley(IEnumerable<string> lines)
    {
        var linesList = lines.ToList();
        Width = linesList.First().Length;
        Height = linesList.Count;
        // Read out blizzards:
        Blizzards = linesList
            .Select(
                (line, y) => line.ToCharArray()
                    .Select((c, x) => (c, x))
                    .Where(item => item.c != '.' && item.c != '#')
                    .Select(item => new Blizzard(item.x, y, item.c))
            )
            .SelectMany(bList => bList)
            .ToList();
        BlizzardPos = GetBlizzardArray();
    }

    public bool[,,] GetBlizzardArray()
    {
        var aHeight = Height - 2;
        var aWidth = Width - 2;
        Lcm = Helpers.Lcm(aHeight, aWidth);
        Console.WriteLine($"Creating array of size ({aHeight}, {aWidth}, {Lcm})");
        var result = new bool[aHeight, aWidth, Lcm];
        foreach (var t in Enumerable.Range(0, Lcm))
        {
            foreach (var i in Enumerable.Range(0, aHeight))
            {
                foreach (var j in Enumerable.Range(0, aWidth))
                {
                    result[i, j, t] = false;
                }
            }
        }
        // Make history for each blizzard:
        foreach (var blizzard in Blizzards)
        {
            foreach (var t in Enumerable.Range(0, Lcm))
            {
                var node = blizzard.PositionAtTime(t, aWidth, aHeight);
                try
                {
                    result[node.Y, node.X, t] = true;
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new IndexOutOfRangeException($"Something went wrong for node {node}", ex);
                }
            }
        }
        return result;
    }

    public int FindShortestPath(Node<int> source, Node<int> target, int startTime = 0)
    {
        // Initialize stuff:
        var queue = new Queue<(Node<int> pos, int dist)>();
        queue.Enqueue((source, startTime));
        var explored = new HashSet<Node<int>>();
        int currentDist = -1;
        while (queue.TryDequeue(out var pair))
        {
            var (pos, dist) = pair;
            if (pos == target) return dist;
            if (dist > currentDist)
            {
                explored = new HashSet<Node<int>>();
                currentDist = dist;
            }
            //explored.Add(pos);
            // Get adjacent nodes:
            foreach (var w in GetAdjacentNodes(pos, dist + 1))
            {
                if (!explored.Contains(w))
                {
                    explored.Add(w);
                    queue.Enqueue((w, dist + 1));
                }
            }
        }
        Console.Write("\n");
        throw new Exception($"Couldn't find target node.");
    }

    public IEnumerable<Node<int>> GetAdjacentNodes(Node<int> pos, int time)
    {
        foreach (var dir in AdjacentDirections)
        {
            var node = pos + dir;
            // Check boundaries:
            if (node.X < 1 || node.X >= Width - 1) continue;
            if (node.Y < 1)
            {
                if (node.Y != 0 || node.X != 1) continue;
            }
            if (node.Y >= Height - 1)
            {
                if (node.Y != Height - 1 || node.X != Width - 2) continue;
            }
            // Check if blizzards are blocking:
            if (node.Y > 0 && node.Y < Height - 1 && BlizzardPos[node.Y - 1, node.X - 1, time % Lcm]) continue;
            yield return node;
        }
    }

    public static readonly IEnumerable<(int x, int y)> AdjacentDirections =
        new List<(int x, int y)> { (0, 0), (1, 0), (-1, 0), (0, 1), (0, -1) };


    public void PrintValley()
    {
        // Print top row:
        Console.Write("#.");
        Console.Write(string.Join("", Enumerable.Repeat("#", Width - 2)));
        Console.Write("\n");
        foreach (var y in Enumerable.Range(1, Height - 2))
        {
            Console.Write("#");
            foreach (var x in Enumerable.Range(1, Width - 2))
            {
                var node = new Node<int>
                {
                    X = x,
                    Y = y
                };
                // Check if we're at a blizzard:
                var curBliz = Blizzards
                    .Where(b => b.Position == node);
                if (curBliz.Count() == 1)
                {
                    Console.Write(curBliz.First().GetChar());
                }
                else if (curBliz.Count() > 1)
                {
                    Console.Write(curBliz.Count());
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.Write("#\n");
        }
        Console.Write(string.Join("", Enumerable.Repeat("#", Width - 2)));
        Console.Write(".#");
        Console.Write("\n");
    }
}