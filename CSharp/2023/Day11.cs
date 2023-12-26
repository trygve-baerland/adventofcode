using AoC.Utils;

namespace AoC.Y2023;

public sealed class Day11 : IPuzzle
{
    private GalaxyMap TestData = new GalaxyMap( "2023/inputdata/day11_test.txt".GetLines().Select( l => l.ToCharArray() ).ToArray() );
    private GalaxyMap ActualData = new GalaxyMap( "2023/inputdata/day11.txt".GetLines().Select( l => l.ToCharArray() ).ToArray() );

    public void Part1()
    {
        var map = ActualData;
        var galaxies = map.Galaxies;
        // Go through each pair:
        var result = ActualData.Galaxies
            .Pairs()
            .Select( p => map.Distance( p.Item1, p.Item2, dilation: 2 ) )
            .Sum();

        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ActualData;
        var galaxies = map.Galaxies;
        // Go through each pair:
        var result = ActualData.Galaxies
            .Pairs()
            .Select( p => map.Distance( p.Item1, p.Item2, dilation: 1000000 ) )
            .Sum();

        Console.WriteLine( result );

    }
}


internal record class GalaxyMap( char[][] map ) : CharMap( map )
{
    private HashSet<Node2D<int>>? _galaxies = null;
    public HashSet<Node2D<int>> Galaxies
    {
        get {
            if ( _galaxies == null )
            {
                _galaxies = new HashSet<Node2D<int>>( Data.SelectMany( ( r, x ) => r.Select( ( c, y ) => (x, y, c) ) )
                    .Where( rc => rc.c == '#' )
                    .Select( rc => new Node2D<int>( rc.x, rc.y ) ) );
            }
            return _galaxies;
        }
    }

    public long Distance( Node2D<int> p1, Node2D<int> p2, long dilation = 2 )
    {
        var dx = System.Math.Sign( p2.X - p1.X );
        var dy = System.Math.Sign( p2.Y - p1.Y );
        // Traverse rows:
        var x = p1.X;
        long distance = 0;
        while ( x != p2.X )
        {
            if ( !Row( x ).Contains( '#' ) )
            {
                distance += dilation;
            }
            else
            {
                distance++;
            }
            x += dx;
        }

        // Traverse columns:
        var y = p1.Y;
        while ( y != p2.Y )
        {
            if ( !Column( y ).Contains( '#' ) )
            {
                distance += dilation;
            }
            else
            {
                distance++;
            }
            y += dy;
        }
        return distance;
    }
}
