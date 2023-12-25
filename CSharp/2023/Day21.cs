using AoC.Utils;
namespace AoC.Y2023;

public sealed class Day21 : IPuzzle
{
    public GardenMap TestMap() => GardenMap.FromFile( "2023/inputdata/day21_test.txt" );
    public GardenMap ActualMap() => GardenMap.FromFile( "2023/inputdata/day21.txt" );

    public void Part1()
    {
        var map = ActualMap();
        var result = map.DoStepsFrom( map.Start, 64 );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualMap();
        var (midx, midy) = (map.Height / 2, map.Width / 2);
        var L = map.Height; // 131 for the actual map

        // Get number of plots in the center tile:
        var parity = 26501365 % 2;
        long result = 0L;

        var N = (26501365 - midx) / L;

        // Number of tiles with same cirality
        long numEvenTiles = Enumerable.Range( 1, N - 1 ) // 1, 2, 3, ..., N - 1
            .Where( i => i.IsEven() )
            .Select( i => Utils.Math.ManhattanRadius( i ) )
            .Sum() + 1;
        result += numEvenTiles * map.DoStepsFrom( new Point( midx, midy ), 2 * L + parity );

        long numOddTiles = Enumerable.Range( 1, N - 1 )
            .Where( i => !i.IsEven() )
            .Select( i => Utils.Math.ManhattanRadius( i ) )
            .Sum();
        result += numOddTiles * map.DoStepsFrom( new Point( midx, midy ), 2 * L + (1 - parity) );

        // Now we need to add up everything around the border:
        // The corner tiles:
        // L - 1 is even, and we've spent N-1 * 131 steps to get here.
        // So if atBorder is true we spent an even number of steps to get here.

        result += map.MidPoints()
            .Select( p => map.DoStepsFrom( p, L - 1 ) )
            .Sum();

        // Now we fill in the small corners
        // L / 2 - 1 = 64, which is even
        result += N * map.CornerPoints()
            .Select( p => map.DoStepsFrom( p, L / 2 - 1 ) )
            .Sum();

        // Fill in the large corners
        // 131 + 64 = 195, which is odd
        // Meaning we've spent an even number of steps getting here.
        // So the starting point should not be included
        result += (N - 1) * map.CornerPoints()
            .Select( p => map.DoStepsFrom( p, L + L / 2 - 1 ) )
            .Sum();
        Console.WriteLine( $"Final result: {result}" );

    }
}

public record struct GardenMap( char[][] Map )
{
    public int Height => Map.Length;
    public int Width => Map[0].Length;

    public char this[Point p] => Map[p.X][p.Y];

    public static GardenMap FromFile( string filename )
    {
        return new GardenMap( filename.GetLines().Select( line => line.ToCharArray() ).ToArray() );
    }

    public override string ToString()
    {
        return string.Join( "\n", Map.Select( line => new string( line ) ) );
    }

    public long DoStepsFrom( Point start, int numSteps )
    {
        var queue = new PriorityQueue<Point, int>();
        var parity = numSteps.IsEven();
        queue.Enqueue( start, 0 );
        var seen = new HashSet<Point>();
        long result = 0;
        while ( queue.TryDequeue( out var p, out var d ) )
        {
            if ( d > numSteps )
            {
                break;
            }
            foreach ( var next in p.Neighbours().Where( Contains ) )
            {
                if ( !seen.Contains( next ) )
                {
                    if ( (d + 1).IsEven() == parity )
                    {
                        result += 1;
                    }
                    seen.Add( next );
                    queue.Enqueue( next, d + 1 );
                }
            }
        }
        return result;
    }

    public bool Contains( Point p ) =>
        p.X >= 0 && p.X < Height && p.Y >= 0 && p.Y < Width && this[p] != '#';

    public Point Find( char c ) =>
        Map
        .SelectMany( ( line, x ) => line.Select( ( ch, y ) => (ch, x, y) ) )
        .Where( t => t.ch == c )
        .Select( t => new Point( t.x, t.y ) )
        .First();

    public Point Start => Find( 'S' );

    public IEnumerable<Point> MidPoints()
    {
        yield return new Point( Height / 2, 0 );
        yield return new Point( Height / 2, Width - 1 );
        yield return new Point( 0, Width / 2 );
        yield return new Point( Height - 1, Width / 2 );
    }

    public IEnumerable<Point> CornerPoints()
    {
        yield return new Point( 0, 0 );
        yield return new Point( 0, Width - 1 );
        yield return new Point( Height - 1, 0 );
        yield return new Point( Height - 1, Width - 1 );
    }
}
