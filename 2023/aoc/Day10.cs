using Utils;

namespace AoC;

public sealed class Day10 : IPuzzle
{
    public PipeMap TestMap { get; } = new PipeMap( "inputdata/day10_test.txt".Stream().GetLines().Select( line => line.ToCharArray() ).ToArray() );
    public PipeMap ActualMap { get; } = new PipeMap( "inputdata/day10.txt".Stream().GetLines().Select( line => line.ToCharArray() ).ToArray() );
    public void Part1()
    {
        var map = ActualMap;
        var start = map.GetStart();
        // Let's set up for BFS:
        var queue = new Queue<(int distance, Point p)>();
        var visited = new HashSet<Point>();
        var maxDistance = 0;
        queue.Enqueue( (0, start) );
        while ( queue.Count > 0 )
        {
            var (distance, p) = queue.Dequeue();
            maxDistance = System.Math.Max( maxDistance, distance );
            // Set point as visisted:
            visited.Add( p );
            // Get all directions from the current point:
            foreach ( var n in map.ConnectedPipes( p, map[p] ) )
            {
                // If we haven't visited this point before, add it to the queue:
                if ( !visited.Contains( n ) )
                {
                    queue.Enqueue( (distance + 1, n) );
                }
            }
        }
        Console.WriteLine( maxDistance );
    }

    public void Part2()
    {
        // Get points constituting the loop:
        var map = ActualMap;
        var start = map.GetStart();
        var loop = new List<Point> {
            start
        };
        var current = start;
        bool returned = false;
        long area = 0;
        long circumference = 0;
        while ( !returned )
        {
            Point next = map.ConnectedPipes( current, map[current] ).Where( p => !loop.Contains( p ) ).FirstOrDefault( defaultValue: current );
            // We know have an edge:
            area += current.Cross( next );
            circumference += 1;
            if ( next != current )
            {
                loop.Add( next );
                current = next;
            }
            else
            {
                returned = true;
            }
        }
        area += current.Cross( start );
        area = System.Math.Abs( area ) / 2;
        circumference = circumference / 2;
        var result = area - circumference + 1;
        Console.WriteLine( result );

    }
}

public struct Point( int x, int y )
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public Point Down() => new Point( X + 1, Y );
    public Point Up() => new Point( X - 1, Y );
    public Point Left() => new Point( X, Y - 1 );
    public Point Right() => new Point( X, Y + 1 );

    public (int x, int y) ToTuple() => (X, Y);
    public static (int x, int y) operator -( Point p1, Point p2 ) => (p1.X - p2.X, p1.Y - p2.Y);
    public static bool operator ==( Point p1, Point p2 ) => p1.X == p2.X && p1.Y == p2.Y;
    public static bool operator !=( Point p1, Point p2 ) => !(p1 == p2);
    public int Cross( Point p ) => X * p.Y - Y * p.X;
}

public class PipeMap( char[][] map )
{
    public char[][] Map { get; } = map;
    private int NC { get; } = map[0].Length;
    private int NR { get; } = map.Length;

    public char this[Point p] => Map[p.X][p.Y];

    private char? sVal = null;
    public char SVal
    {
        get {
            if ( sVal == null )
            {
                var start = GetStart();
                sVal = MapS( start );
            }
            return sVal.Value;
        }
    }

    public IEnumerable<Point> Points() =>
        Enumerable.Range( 0, NR )
        .SelectMany(
            i => Enumerable.Range( 0, NC )
                .Select( j => new Point( i, j ) )
        );

    public char MapS( Point p )
    {
        // Assuming the given point is S, find out what letter it has replaced.
        var neighbours = Directions( p ).Where( d => this[d] != '.' && ConnectedPipes( d, this[d] ).Contains( p ) ).ToArray();

        if ( neighbours.Length != 2 )
            throw new Exception( $"Expected 2 connecting neighbours, found {neighbours.Length}" );
        return (neighbours[1] - neighbours[0]) switch {
            (1, 1 ) => 'L',
            (1, -1 ) => 'F',
            (-1, -1 ) => '7',
            (-1, 1 ) => 'J',
            (0, -2 ) => '-',
            (2, 0 ) => '|',
            _ => throw new Exception( $"Unknown pipe type: {neighbours[1] - neighbours[0]}" )
        };
    }

    public Point GetStart()
    {
        return Map.Select(
            ( row, i ) => (i, row.Select(
                ( col, j ) => (col, j)
            )
            .Where( t => t.col == 'S' )
        ) )
        .Where( item => item.Item2.Any() )
        .Select( item => new Point( item.Item1, item.Item2.First().j ) )
        .First();
    }

    public IEnumerable<Point> Directions( Point p )
    {
        if ( p.X > 0 )
            yield return p.Up();
        if ( p.Y < NC - 1 )
            yield return p.Right();
        if ( p.X < NR - 1 )
            yield return p.Down();
        if ( p.Y > 0 )
            yield return p.Left();
    }

    public IEnumerable<Point> ConnectedPipes( Point p, char val )
    {
        switch ( val )
        {
            case 'F':
                if ( p.X < NR - 1 ) yield return p.Down();
                if ( p.Y < NC - 1 ) yield return p.Right();
                break;
            case 'L':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.Y < NC - 1 ) yield return p.Right();
                break;
            case 'J':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.Y > 0 ) yield return p.Left();
                break;
            case '7':
                if ( p.X < NR - 1 ) yield return p.Down();
                if ( p.Y > 0 ) yield return p.Left();
                break;
            case '|':
                if ( p.X > 0 ) yield return p.Up();
                if ( p.X < NR - 1 ) yield return p.Down();
                break;
            case '-':
                if ( p.Y > 0 ) yield return p.Left();
                if ( p.Y < NC - 1 ) yield return p.Right();
                break;
            case 'S':
                foreach ( var pipe in ConnectedPipes( p, SVal ) )
                {
                    yield return pipe;
                }
                break;
            default:
                throw new Exception( $"Unknown pipe type: {val}" );
        }
    }
}

public static partial class Helpers
{
    public static int WindingNumber( List<Point> loop, Point x )
    {
        // We need to iterate over all edges in the loop.
        return ( int ) System.Math.Round( loop.Zip( loop.Repeat().Skip( 1 ) )
            .Select( edge => Angle( edge.Item1 - x, edge.Item2 - x ) )
            .Sum() / (2 * System.Math.PI) );
    }

    public static (double x, double y) Normal( (int x, int y) p )
    {
        var (x, y) = p;
        var l = System.Math.Sqrt( x * x + y * y );
        return (x / l, y / l);
    }

    public static double Dot( (double x, double y) p1, (double x, double y) p2 ) => p1.x * p2.x + p1.y * p2.y;

    public static double Cross( (double x, double y) p1, (double x, double y) p2 ) => p1.x * p2.y - p1.y * p2.x;

    public static double Angle( (int x, int y) p1, (int x, int y) p2 )
    {
        var n1 = Normal( p1 );
        var n2 = Normal( p2 );

        var theta = System.Math.Sign( Cross( n1, n2 ) ) * System.Math.Acos( Dot( n1, n2 ) );
        // Now, we need to figure out if the angle is positive or negative.
        return theta;
    }

}
