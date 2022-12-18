using Utils;
using Utils.Graphs;
namespace Day17;

public enum Move
{
    Right,
    Left,
    Down,
}

public class Board
{
    public int Width { get; set; }
    private HashSet<Node<long>> Blockers { get; } = new();
    public long Height { get => Blockers.Any() ? Blockers.Select(node => -node.Y).Max() : 0; }

    public void AddBlock(Node<long> node, Shape shape)
    {
        foreach (var point in shape.GetNodes(node))
        {
            Blockers.Add(point);
        }
    }

    public void InsertShape(Shape shape, IEnumerator<Move> moves)
    {
        // Get starting position:
        var pos = new Node<long>
        {
            X = 2,
            Y = -Height - 4
        };
        /*
        // We use DFS Iterative to get to where we need to be:
        GraphSearch.DFSIterative<(Node<long> node, Shape shape)>(
            source: (pos, shape),
            adjacentNodes: state => GetMoves(state, moves),
            postVisitor: state =>
            {
                AddBlock(state.node, state.shape);
                return true;
            }
        );
        */
        while (moves.MoveNext())
        {
            // Move left or right:
            var move = moves.Current;
            (int x, int y) dir = move switch
            {
                Move.Left => (-1, 0),
                Move.Right => (1, 0),
                _ => throw new Exception($"Invalid move '{move}'")
            };
            var candPos = pos + dir;
            if (IsFree(candPos, shape)) pos = candPos;

            // Move down:
            candPos = pos + (0, 1);
            if (IsFree(candPos, shape)) pos = candPos;
            else // We are done;
            {
                AddBlock(pos, shape);
                break;
            }
        }
    }

    public IEnumerable<(Node<long> node, Shape shape)> GetMoves((Node<long> node, Shape shape) state, IEnumerator<Move> moves)
    {
        //
        if (moves.MoveNext())
        {
            var move = moves.Current;
            (int x, int y) dir = move switch
            {
                Move.Left => (-1, 0),
                Move.Right => (1, 0),
                _ => throw new Exception($"Invalid move '{move}'")
            };
            // We first try diagonal
            var candidate = state.node + (dir.x, 1);
            if (IsFree(candidate, state.shape))
                yield return (candidate, state.shape);
            // Then we try directly down:
            candidate = state.node + (0, 1);
            if (IsFree(candidate, state.shape))
                yield return (candidate, state.shape);
            // Next we try just to the left or right
            candidate = state.node + dir;
            if (IsFree(candidate, state.shape))
                yield return (candidate, state.shape);
        }
    }

    public bool IsFree(Node<long> node, Shape shape)
    {
        return node.X >= 0 && node.X + shape.Width - 1 < Width && node.Y < 0 &&
            shape.GetNodes(node).All(point => !Blockers.Contains(point));
    }

    public void Print()
    {
        var height = Height;
        for (long rowi = -height; rowi < 0; rowi++)
        {
            Console.Write("|");
            foreach (var coli in Enumerable.Range(0, Width))
            {
                if (Blockers.Contains(new Node<long> { X = coli, Y = rowi }))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine("|");
        }
        // Floor:
        Console.Write("|");
        foreach (var _ in Enumerable.Range(0, Width))
        {
            Console.Write("-");
        }
        Console.WriteLine("|");
    }
}

public class Shape
{
    public List<(long x, long y)> Offsets { get; set; } = new();
    public int Width { get; set; }

    public IEnumerable<Node<long>> GetNodes(Node<long> node) => Offsets.Select(offset => node + offset);

    public static readonly List<Shape> Shapes = new() {
        // Horizontal line:
        // ####
        new Shape () {
            Offsets = new() {(0,0), (1,0), (2,0), (3,0)},
            Width = 4
        },
        // Cross:
        //    #
        //   ###
        //    #
        new Shape (){
            Offsets = new() {(1,0), (0,-1), (1, -1), (2, -1), (1, -2)},
            Width = 3
        },
        // Mirrored L
        //     #
        //     #
        //   ###
        new Shape() {
            Offsets = new() {(0,0), (1,0), (2,0), (2,-1), (2,-2)},
            Width = 3
        },
        // Vertical line
        //   #
        //   #
        //   #
        //   #
        new Shape() {
            Offsets = new() {(0, 0), (0, -1), (0, -2), (0, -3)},
            Width = 1
        },
        // Square
        //   ##
        //   ##
        new Shape() {
            Offsets = new() {(0,0), (1,0), (0, -1), (1, -1)},
            Width = 2
        }
    };
}