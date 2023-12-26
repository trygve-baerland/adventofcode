using System.Diagnostics;
using AoC.Utils;
namespace AoC.Y2022;

public sealed class Day12 : IPuzzle
{
    private Terrain TestTerrain() => Terrain.FromFile( "2022/inputdata/day12_test.txt" );
    private Terrain ActualTerrain() => Terrain.FromFile( "2022/inputdata/day12.txt" );
    public void Part1()
    {
        var terrain = ActualTerrain();
        // Find source and target:
        var S = terrain.Find( 'S' );
        var E = terrain.Find( 'E' );

        var result = ShortestPath.BFS( S, E,
            adjacentNodes: v => terrain.AdjacentNodes( v, noHeight: false )
        );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var terrain = ActualTerrain();
        // Find target:
        var E = terrain.Find( 'E' );
        var result = Graph.BFS(
            source: E,
            adjacentNodes: v =>
                v.Neighbours()
                .Where( w => terrain.Contains( w ) && terrain.CanDescend( v, w ) )
        ).First( item => Terrain.MapChar( terrain[item.node] ) == 'a' ).dist;
        Console.WriteLine( result );
    }
}

internal record class Terrain( char[][] data ) : CharMap( data )
{
    public new static Terrain FromFile( string filename ) =>
        new( filename.GetLines().Select( line => line.ToCharArray() ).ToArray() );

    public static char MapChar( char c ) => c switch {
        'S' => 'a',
        'E' => 'z',
        _ => c
    };

    public bool CanTraverse( Node2D<int> source, Node2D<int> target )
    {
        char s = MapChar( this[source] );
        char t = MapChar( this[target] );
        return t - s <= 1;
    }

    public bool CanDescend( Node2D<int> source, Node2D<int> target )
    {
        char s = MapChar( this[source] );
        char t = MapChar( this[target] );
        return t - s >= -1;
    }

    public IEnumerable<Node2D<int>> AdjacentNodes( Node2D<int> v, bool noHeight ) =>
        v.Neighbours().Where( w => Contains( w ) && (noHeight || CanTraverse( v, w )) );

    public int EdgeWeight( Node2D<int> source, Node2D<int> target )
    {
        char s = MapChar( this[source] );
        char t = MapChar( this[target] );
        if ( t - s > 1 )
        {
            return int.MaxValue;
        }
        else
        {
            return 1;
        }
    }

    public int GetCandidateDistance( int oldDistance, int edgeWeight )
    {
        try
        {
            return checked(oldDistance + edgeWeight);
        }
        catch ( OverflowException )
        {
            return int.MaxValue;
        }
    }
}
