using System.Text;
using Utils;
namespace AoC;

public sealed class Day23 : IPuzzle
{
    public PathMap TestMap() => PathMap.FromFile( "inputdata/day23_test.txt" );
    public PathMap ActualMap() => PathMap.FromFile( "inputdata/day23.txt" );

    public void Part1()
    {
        var map = ActualMap();
        var graph = map.Condense();
        var start = new Point( 0, 1 );
        var end = new Point( map.Height - 1, map.Width - 2 );
        var result = graph.AllPaths( start, end, directed: true ).Max();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualMap();
        var graph = map.Condense();
        var start = new Point( 0, 1 );
        var end = new Point( map.Height - 1, map.Width - 2 );
        var result = graph.AllPaths( start, end, directed: false ).Max();
        Console.WriteLine( result );
    }
}

public record class PathMap( char[][] Map )
{
    public int Height { get; } = Map.Length;
    public int Width { get; } = Map[0].Length;
    public char this[Point p] => Map[p.X][p.Y];

    public static PathMap FromFile( string filename ) =>
        new PathMap(
            filename
            .GetLines()
            .Select( line => line.ToCharArray() )
            .ToArray()
        );

    public bool Contains( Point p ) =>
        p.X >= 0 && p.X < Height && p.Y >= 0 && p.Y < Width && this[p] != '#';



    public CondensedGraph Condense()
    {
        var result = new CondensedGraph();
        var start = new Point( 0, 1 );
        // Add start and end points:
        result.Add( start );
        // Add junction points:
        Junctions().ForEach( result.Add );
        result.Add( new Point( Height - 1, Width - 2 ) );

        // Add edges:
        // We do this as a BFS search.
        var queue = new Queue<(Point p, Point node, int length)>();
        var seen = new HashSet<Point>();
        queue.Enqueue( (start, start, 0) );

        while ( queue.TryDequeue( out var item ) )
        {
            var (p, node, length) = item;

            foreach ( var next in SlopeNeighbours( p ) )
            {
                if ( next != node && result.Contains( next ) )
                {
                    // We found a new going from node to next:
                    result[node, next] = length + 1;
                    queue.Enqueue( (next, next, 0) );
                }
                else if ( !seen.Contains( next ) )
                {
                    seen.Add( next );
                    queue.Enqueue( (next, node, length + 1) );
                }
            }
        }

        return result;
    }

    public IEnumerable<Point> Junctions()
        => Enumerable.Range( 0, Height ).SelectMany( x =>
            Enumerable.Range( 0, Width ).Select( y => new Point( x, y ) )
        ).Where( p => isJunction( p ) && Contains( p ) );

    public bool isJunction( Point p ) =>
        p.Neighbours().Count( Contains ) > 2;

    public IEnumerable<Point> SlopeNeighbours( Point p )
    {
        // If we are on a slope, we can only go one way:
        if ( OnSlope( p ) )
        {
            var slopeDir = SlopeDirection( this[p] );
            var next = p + slopeDir;
            if ( Contains( next ) )
            {
                yield return next;
            }
            yield break;
        }
        // Otherwise, we can look at every direction:
        foreach ( var dir in Point.Directions() )
        {
            var next = p + dir;
            // If we are on a slope, we have to have gotten there from the top side:
            if ( Contains( next ) &&
                (!OnSlope( next ) ||
                    dir == SlopeDirection( this[next] )
                )
            )
            {
                yield return next;
            }
        }
    }

    public IEnumerable<Point> NonSlopedNeighbours( Point p ) =>
        p.Neighbours().Where( Contains );

    public static (int x, int y) SlopeDirection( char c ) => c switch {
        '>' => (0, 1),
        '<' => (0, -1),
        '^' => (-1, 0),
        'v' => (1, 0),
        _ => throw new ArgumentException( $"Invalid slope direction: {c}" )
    };

    public bool OnSlope( Point p ) => this[p] switch {
        '>' => true,
        '<' => true,
        '^' => true,
        'v' => true,
        _ => false
    };


}

public class CondensedGraph
{
    public HashSet<Point> Points { get; } = new();
    public Dictionary<(Point, Point), int> Edges { get; } = new();

    public int this[Point p1, Point p2]
    {
        get => Edges[(p1, p2)];
        set => Edges[(p1, p2)] = value;
    }

    public bool Contains( Point p ) => Points.Contains( p );

    public void Add( Point p ) => Points.Add( p );

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine( $"Points: {string.Join( '\n', Points )}" );
        sb.AppendLine( $"Edges: {string.Join( '\n', Edges )}" );
        return sb.ToString();
    }

    public IEnumerable<(Point point, int distance)> DirectedNeighbours( Point p ) =>
        Edges
        .Where( kvp => kvp.Key.Item1 == p )
        .Select( kvp => (kvp.Key.Item2, kvp.Value) );


    public IEnumerable<(Point point, int distance)> UndirectedNeighbours( Point p )
    {
        foreach ( var (p1, p2) in Edges.Keys )
        {
            if ( p1 == p )
            {
                yield return (p2, Edges[(p1, p2)]);
            }
            else if ( p2 == p )
            {
                yield return (p1, Edges[(p1, p2)]);
            }
        }
    }

    public IEnumerable<long> AllPaths( Point start, Point end, bool directed = true )
    {
        Func<Point, IEnumerable<(Point, int)>> neighbours = directed
            ? DirectedNeighbours
            : UndirectedNeighbours;

        var queue = new PriorityQueue<IEnumerable<Point>, long>();
        queue.Enqueue( new[] { start }, 0L );
        while ( queue.TryDequeue( out var path, out var distance ) )
        {
            var last = path.Last();
            if ( last == end )
            {
                yield return distance;
            }
            else
            {
                foreach ( var (next, length) in neighbours( last ) )
                {
                    if ( !path.Contains( next ) )
                    {
                        queue.Enqueue( path.Append( next ), distance + length );
                    }
                }
            }
        }
    }
}
