using AoC.Utils;

namespace AoC.Y2024;

public sealed class Day4 : IPuzzle
{
    private CharMap TestMap = CharMap.FromFile( "2024/inputdata/day4_test.txt" );
    private CharMap ProdMap = CharMap.FromFile( "2024/inputdata/day4.txt" );
    public void Part1()
    {

        long result = 0;
        var map = ProdMap;
        foreach ( var node in map.Where( c => c == 'X' ) )
        {
            foreach ( var dir in Tangent2D<int>.Directions().Concat( Tangent2D<int>.Diagonals() ) )
            {
                if ( MapHelpers.CheckAlong( map, node, dir, "XMAS" ) )
                {
                    result += 1;
                }
            }
        }
        Console.WriteLine( result );
    }

    public void Part2()
    {

        long result = 0;
        var map = ProdMap;
        foreach ( var node in map.Where( c => c == 'M' ) )
        {
            foreach ( var dir in Tangent2D<int>.Diagonals() )
            {
                if ( MapHelpers.CheckAlong( map, node, dir, "MAS" ) )
                {
                    var m = node + (2 * dir.ProjX());
                    var mdir = dir - (2 * dir.ProjX());
                    if (
                        MapHelpers.CheckAlong( map, m, mdir, "MAS" ) ||
                        MapHelpers.CheckAlong( map, m, mdir, "SAM" ) )
                    {
                        result += 1;
                    }
                }
            }
        }
        // We are currently counting everything twice.
        Console.WriteLine( result / 2 );
    }
}

internal static class MapHelpers
{
    public static bool CheckAlong( CharMap map, Node2D<int> node, Tangent2D<int> dir, string goal )
    {
        return string.Concat(
            node.Along( dir )
                .TakeWhile( map.Contains )
                .Take( goal.Length )
                .Select( n => map[n] )
        ) == goal;
    }
}


