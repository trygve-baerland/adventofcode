using System.ComponentModel.DataAnnotations;
using Utils;
namespace Day22;

public enum Rotation
{
    None = 0,
    R = 1,
    Switch = 2,
    L = 3,
}

public enum Direction
{
    R = 0,
    D = 1,
    L = 2,
    U = 3
}

public class Turtle
{
    public Node<int> Position { get; set; }
    public Direction MovingDirection { get; set; }
    public void MoveOnBoard(Board board, int numSteps, bool onCube = false)
    {
        // node enumerator:
        var nodes = onCube switch
        {
            false => board.GetNodesFrom(Position, MovingDirection).GetEnumerator(),
            true => Board.GetNodesOnCube(Position, MovingDirection).GetEnumerator()
        };
        while (nodes.MoveNext() && numSteps > 0)
        {
            var (node, dir) = nodes.Current;
            if (board.GetValue(node) == '.')
            {
                Position = node;
                MovingDirection = dir;
                numSteps--;
            }
            else
            {
                break;
            }
        }
    }

    public void Rotate(Rotation rot)
    {
        MovingDirection = (Direction)(((int)MovingDirection + (int)rot) % 4);
    }

    public override string ToString()
    {
        return $"Turtle at {Position} facing {MovingDirection}";
    }

    public int EvaluatePosition()
    {
        return 1000 * (Position.Y + 1) + 4 * (Position.X + 1) + (int)MovingDirection;
    }
}

public class Board
{
    public char[][] Map { get; init; } = default!;

    private List<char> Valids { get; } = new() { '.', '#' };

    public IEnumerable<(char, int)> GetValidCols(int row) => Map[row]
        .Select((c, i) => (c, i))
        .Where(item => Valids.Contains(item.c));

    public IEnumerable<(char, int)> GetValidRows(int col) => Map
        .Select((row, i) => (row[col], i))
        .Where(item => Valids.Contains(item.Item1));

    public IEnumerable<(Node<int> node, Direction dir)> GetNodesFrom(Node<int> node, Direction dir)
    {
        // Horizontal movement
        if (dir == Direction.R)
        {
            return Enumerable.Range(0, Map[node.Y].Length)
                .Where(x => Valids.Contains(Map[node.Y][x]))
                .Repeat()
                .SkipWhile(x => x != node.X)
                .Skip(1)
                .Select(x => (new Node<int>
                {
                    X = x,
                    Y = node.Y
                }, dir));
        }
        else if (dir == Direction.L)
        {
            return Enumerable.Range(0, Map[node.Y].Length)
                .Where(x => Valids.Contains(Map[node.Y][x]))
                .Reverse()
                .Repeat()
                .SkipWhile(x => x != node.X)
                .Skip(1)
                .Select(x => (new Node<int>
                {
                    X = x,
                    Y = node.Y
                }, dir));
        }
        else if (dir == Direction.D)
        {
            return Enumerable.Range(0, Map.Length)
                .Where(ri => Map[ri].Length > node.X && Valids.Contains(Map[ri][node.X]))
                .Repeat()
                .SkipWhile(ri => ri != node.Y)
                .Skip(1)
                .Select(y => (new Node<int>
                {
                    X = node.X,
                    Y = y
                }, dir));
        }
        else // Up:
        {
            return Enumerable.Range(0, Map.Length)
                .Where(ri => Map[ri].Length > node.X && Valids.Contains(Map[ri][node.X]))
                .Reverse()
                .Repeat()
                .SkipWhile(ri => ri != node.Y)
                .Skip(1)
                .Select(y => (new Node<int>
                {
                    X = node.X,
                    Y = y
                }, dir));
        }
    }

    public static IEnumerable<Node<int>> GetHorizontalLines([Range(0, 49)] int offset, [Range(1, 6)] int blockNumber)
    {
        if (blockNumber == 1)
        {
            return Enumerable.Range(0, 50)
                .Select(x => new Node<int>
                {
                    X = 50 + x,
                    Y = offset
                });
        }
        else if (blockNumber == 2)
        {
            return Enumerable.Range(0, 50)
                .Select(x => new Node<int>
                {
                    X = 100 + x,
                    Y = offset
                });
        }
        else if (blockNumber == 3)
        {
            return Enumerable.Range(0, 50)
                .Select(x => new Node<int>
                {
                    X = 50 + x,
                    Y = 50 + offset
                });
        }
        else if (blockNumber == 4)
        {
            return Enumerable.Range(0, 50)
                .Select(x => new Node<int>
                {
                    X = 50 + x,
                    Y = 100 + offset
                });
        }
        else if (blockNumber == 5)
        {
            return Enumerable.Range(0, 50)
                .Select(x => new Node<int>
                {
                    X = x,
                    Y = 100 + offset
                });
        }
        else // blockNumber == 6
        {
            return Enumerable.Range(0, 50)
                .Select(x => new Node<int>
                {
                    X = x,
                    Y = 150 + offset
                });
        }
    }

    public static IEnumerable<Node<int>> GetVerticalNodes([Range(0, 49)] int offset, [Range(1, 6)] int blockNumber)
    {
        if (blockNumber == 1)
        {
            return Enumerable.Range(0, 50)
                .Select(y => new Node<int>
                {
                    X = 50 + offset,
                    Y = y
                });
        }
        else if (blockNumber == 2)
        {
            return Enumerable.Range(0, 50)
                .Select(y => new Node<int>
                {
                    X = 100 + offset,
                    Y = y
                });
        }
        else if (blockNumber == 3)
        {
            return Enumerable.Range(0, 50)
                .Select(y => new Node<int>
                {
                    X = 50 + offset,
                    Y = 50 + y
                });
        }
        else if (blockNumber == 4)
        {
            return Enumerable.Range(0, 50)
                .Select(y => new Node<int>
                {
                    X = 50 + offset,
                    Y = 100 + y
                });
        }
        else if (blockNumber == 5)
        {
            return Enumerable.Range(0, 50)
                .Select(y => new Node<int>
                {
                    X = offset,
                    Y = 100 + y
                });
        }
        else // blockNumber == 6
        {
            return Enumerable.Range(0, 50)
                .Select(y => new Node<int>
                {
                    X = offset,
                    Y = 150 + y
                });
        }
    }

    public static IEnumerable<(Node<int> node, Direction dir)> GetNodesOnCube(Node<int> node, Direction dir)
    {
        // First, we need to get the block the starting node is contained in:
        var blockNumber = GetBlockNumber(node);
        // Find the line on the cube we are going to follow:
        var lineId = Lines
            .Select((line, i) => (line.Select(pair => (pair.block, pair.dir % 2)).ToList(), i))
            .Where(item => item.Item1.Contains((blockNumber, (int)dir % 2)))
            .Select(item => item.i)
            .First();

        var line = Lines[lineId];
        // Find out if we're supposed to reverse at the the end:
        var intDir = (int)dir;
        var referenceDir = line
            .Where(item => item.block == blockNumber)
            .Select(item => item.dir)
            .First();
        bool shouldReverse = referenceDir != intDir;

        // Get offsets needed:
        var (x, y) = GetOffset(node, blockNumber);
        var offset = dir switch
        {
            Direction.U => x,
            Direction.D => x,
            Direction.R => y,
            Direction.L => y,
            _ => throw new Exception($"Unsupported direction '{dir}'")
        };

        // we're finally in an OK position to collection lines across the cube:
        List<IEnumerable<(Node<int> node, Direction dir)>> resultList = new();
        // Let's make the correct line for each block in the line:
        foreach (var pair in line)
        {
            // Todo: 
            // 1: We need to figure out whether to use offset or 50 - offset
            int curOffset;
            if (lineId == 0)
            {
                curOffset = Math.Abs(referenceDir - pair.dir) > 0 ? 49 - offset : offset;
            }
            else
            {
                curOffset = offset;
            }
            // 2: The rotation also needs to be correct:
            Direction curDir = (Direction)((pair.dir + Math.Abs(referenceDir - intDir)) % 4);

            var blockNodes = pair.dir switch
            {
                0 => GetHorizontalLines(curOffset, pair.block),
                2 => GetHorizontalLines(curOffset, pair.block).Reverse(),
                3 => GetVerticalNodes(curOffset, pair.block).Reverse(),
                _ => throw new Exception($"Got unexpected direction '{pair.dir}'")
            };
            resultList.Add(blockNodes.Select(node => (node, curDir)));
        }
        // Finalize:
        var result = resultList.Aggregate((acc, item) => acc.Chain(item));
        // reverse if needed:
        if (shouldReverse) result = result.Reverse();
        // Repeat indefinitly
        result = result.Repeat();
        return result.SkipWhile(item => item.node != node).Skip(1);
    }

    public static readonly List<List<(int block, int dir)>> Lines = new() {
        new List<(int, int)> {(1,0), (2, 0), (4, 2), (5,2)}, // Last two have 50 - offset as offset
        new List<(int, int)> {(3,0), (2, 3), (6, 3), (5, 3)}, // No offset changes
        new List<(int, int)> {(6,0), (4,3), (3, 3), (1,3)} // No offset changes
    };

    public static int GetBlockNumber(Node<int> node)
    {
        if (node.X >= 50 && node.X < 100 && node.Y >= 0 && node.Y < 50) return 1;
        if (node.X >= 100 && node.X < 150 && node.Y >= 0 && node.Y < 50) return 2;
        if (node.X >= 50 && node.X < 100 && node.Y >= 50 && node.Y < 100) return 3;
        if (node.X >= 50 && node.X < 100 && node.Y >= 100 && node.Y < 150) return 4;
        if (node.X >= 0 && node.X < 50 && node.Y >= 100 && node.Y < 150) return 5;
        if (node.X >= 0 && node.X < 50 && node.Y >= 150 && node.Y < 200) return 6;
        throw new Exception($"Node {node} is not in any block.");
    }

    public static (int x, int y) GetOffset(Node<int> node, int blockNumber)
    {
        if (blockNumber == 1) return (node.X - 50, node.Y - 0);
        if (blockNumber == 2) return (node.X - 100, node.Y - 0);
        if (blockNumber == 3) return (node.X - 50, node.Y - 50);
        if (blockNumber == 4) return (node.X - 50, node.Y - 100);
        if (blockNumber == 5) return (node.X - 0, node.Y - 100);
        if (blockNumber == 6) return (node.X - 0, node.Y - 150);
        throw new Exception($"Invalid block number '{blockNumber}'");
    }

    public char GetValue(Node<int> node)
    {
        try
        {
            return Map[node.Y][node.X];
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new IndexOutOfRangeException($"Cannot get value for node {node}", ex);
        }
    }
}

public static class TracingHelpers
{
    public static (int x, int y) Dx(Direction dir)
    {
        return dir switch
        {
            Direction.U => (0, -1),
            Direction.D => (0, 1),
            Direction.L => (-1, 0),
            Direction.R => (1, 0),
            _ => throw new Exception("...")
        };
    }
}
