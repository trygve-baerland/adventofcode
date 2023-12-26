using AoC.Utils;
namespace AoC.Y2023;

public sealed class Day21 : IPuzzle
{
    private GardenMap TestMap() => GardenMap.FromFile( "2023/inputdata/day21_test.txt" );
    private GardenMap ActualMap() => GardenMap.FromFile( "2023/inputdata/day21.txt" );

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
        result += numEvenTiles * map.DoStepsFrom( new Node2D<int>( midx, midy ), 2 * L + parity );

        long numOddTiles = Enumerable.Range( 1, N - 1 )
            .Where( i => !i.IsEven() )
            .Select( i => Utils.Math.ManhattanRadius( i ) )
            .Sum();
        result += numOddTiles * map.DoStepsFrom( new Node2D<int>( midx, midy ), 2 * L + (1 - parity) );

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

internal record class GardenMap( char[][] Map ) : CharMap( Map )
{

    public static new GardenMap FromFile( string filename ) =>
        new GardenMap( filename.GetLines().Select( line => line.ToCharArray() ).ToArray() );

    public override string ToString()
    {
        return string.Join( "\n", Map.Select( line => new string( line ) ) );
    }

    public long DoStepsFrom( Node2D<int> start, int numSteps )
    {
        var queue = new PriorityQueue<Node2D<int>, int>();
        var parity = numSteps.IsEven();
        queue.Enqueue( start, 0 );
        var seen = new HashSet<Node2D<int>>();
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

    public override bool Contains( Node2D<int> p ) =>
        base.Contains( p ) && this[p] != '#';

    public Node2D<int> Find( char c ) =>
        Map
        .SelectMany( ( line, x ) => line.Select( ( ch, y ) => (ch, x, y) ) )
        .Where( t => t.ch == c )
        .Select( t => new Node2D<int>( t.x, t.y ) )
        .First();

    public Node2D<int> Start => Find( 'S' );

    public IEnumerable<Node2D<int>> MidPoints()
    {
        yield return new Node2D<int>( Height / 2, 0 );
        yield return new Node2D<int>( Height / 2, Width - 1 );
        yield return new Node2D<int>( 0, Width / 2 );
        yield return new Node2D<int>( Height - 1, Width / 2 );
    }

    public IEnumerable<Node2D<int>> CornerPoints()
    {
        yield return new Node2D<int>( 0, 0 );
        yield return new Node2D<int>( 0, Width - 1 );
        yield return new Node2D<int>( Height - 1, 0 );
        yield return new Node2D<int>( Height - 1, Width - 1 );
    }
}
