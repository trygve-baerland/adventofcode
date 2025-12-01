using System.ComponentModel;
using AoC.Utils;

namespace AoC.Y2022;

public sealed class Day8 : IPuzzle
{
    private TerrainMap TestTerrain() =>
        TerrainMap.FromFile( "2022/inputdata/day8_test.txt" );
    private TerrainMap ActualTerrain() =>
        TerrainMap.FromFile( "2022/inputdata/day8.txt" );

    public void Part1()
    {
        var terrain = ActualTerrain();
        var result = terrain.Coordinates()
            .Where( v => terrain.Visible( v ) )
            .Count();

        Console.WriteLine( result );
    }

    public void Part2()
    {
        var terrain = ActualTerrain();
        var result = terrain.Coordinates()
            .Select( terrain.Scenic )
            .Max();
        Console.WriteLine( result );
    }
}

internal record class TerrainMap( char[][] Data ) : CharMap( Data )
{
    public new static TerrainMap FromFile( string path ) =>
        new( path.GetLines().Select( line => line.ToCharArray() ).ToArray() );
    public IEnumerable<Node2D<int>> GetAllInDirection( Node2D<int> v, Tangent2D<int> dir )
    {
        v += dir;
        while ( Contains( v ) )
        {
            yield return v;
            v += dir;
        }
    }
    public bool VisibleInDirection( Node2D<int> v, Tangent2D<int> dir )
    {
        return !GetAllInDirection( v, dir )
            .Where( w => this[w] >= this[v] )
            .Any();
    }

    public bool Visible( Node2D<int> v )
    {
        return Tangent2D<int>.Directions()
            .Where( dir => VisibleInDirection( v, dir ) )
            .Any();
    }

    public int Scenic( IEnumerable<Node2D<int>> sightLine, char height )
    {
        return sightLine.Select( ( tree, index ) => (tree, index) )
            .Where( item => this[item.tree] >= height )
            .Select( item => item.index + 1 )
            .FirstOrDefault( sightLine.Count() );
    }

    public int Scenic( Node2D<int> v )
    {
        var height = this[v];
        return Tangent2D<int>.Directions()
            .Select( dir => GetAllInDirection( v, dir ) )
            .Select( sightLine => Scenic( sightLine, height ) )
            .Aggregate( ( acc, item ) => acc * item );
    }
}
