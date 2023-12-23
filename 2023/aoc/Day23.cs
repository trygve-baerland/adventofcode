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
        var result = graph.LongestPath( true );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualMap();
        var graph = map.Condense();
        var result = graph.LongestPath( false );
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
                    var nextid = result[next];
                    var nodeid = result[node];
                    result[nodeid, nextid] = length + 1;
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
    public Dictionary<Point, long> Points { get; } = new();
    public Dictionary<(long start, long end), int> Edges { get; } = new();

    public int this[long p1, long p2]
    {
        get => Edges[(p1, p2)];
        set => Edges[(p1, p2)] = value;
    }

    public long this[Point p] => Points[p];

    public long Start => Points.Select( kvp => kvp.Value ).Min();
    public long End => Points.Select( kvp => kvp.Value ).Max();

    public bool Contains( Point p ) => Points.ContainsKey( p );
    public void Add( Point p ) => Points.Add( p, 1L << Points.Count );

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine( $"Points: {string.Join( '\n', Points )}" );
        sb.AppendLine( $"Edges: {string.Join( '\n', Edges )}" );
        return sb.ToString();
    }

    public IEnumerable<(long pid, int distance)> DirectedNeighbours( long pid ) =>
        Edges
        .Where( kvp => kvp.Key.Item1 == pid )
        .Select( kvp => (kvp.Key.Item2, kvp.Value) );


    public IEnumerable<(long pid, int distance)> UndirectedNeighbours( long pid ) =>
        Edges.Where( kvp => kvp.Key.Item1 == pid || kvp.Key.Item2 == pid )
        .Select( kvp => (kvp.Key.Item1 == pid ? kvp.Key.Item2 : kvp.Key.Item1, kvp.Value) );


    public long LongestPath( bool directed = true )
    {
        var start = Start;
        var end = End;
        var cache = new Dictionary<(long pid, long visited), long>();
        Func<long, IEnumerable<(long pid, int distance)>> neighbours = directed ? DirectedNeighbours : UndirectedNeighbours;
        return _LongestPath( cache, start, 0L, end, neighbours );
    }

    public long _LongestPath(
        Dictionary<(long pid, long visited), long> cache,
        long fromId,
        long visited,
        long goalId,
        Func<long, IEnumerable<(long pid, int distance)>> neighbours
    )
    {
        if ( fromId == goalId )
        {
            return 0L;
        }
        else if ( (visited & fromId) != 0 ) // Already visited
        {
            return long.MinValue;
        }
        var key = (fromId, visited);
        if ( !cache.ContainsKey( key ) )
        {
            //cache[key] = 
            long max = long.MinValue;
            var nexts = neighbours( fromId ).ToList();
            // If we are at the end, we can only go to the end:
            if ( nexts.Any( t => t.pid == goalId ) )
            {
                nexts = nexts.Where( t => t.pid == goalId ).ToList();
            }
            foreach ( var item in nexts )
            {
                var (nextId, distance) = item;
                max = System.Math.Max( max, distance + _LongestPath( cache, nextId, visited | fromId, goalId, neighbours ) );
            }
            cache[key] = max;
        }
        return cache[key];
    }
}