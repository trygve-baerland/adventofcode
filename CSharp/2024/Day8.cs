
using AoC.Utils;

namespace AoC.Y2024;

public sealed class Day8 : IPuzzle
{
    private readonly CharMap TestAntennas =
        CharMap.FromFile( "2024/inputdata/day8_test.txt" );

    private readonly CharMap ProdAntennas =
        CharMap.FromFile( "2024/inputdata/day8.txt" );
    public void Part1()
    {
        var antennas = ProdAntennas;
        var result = antennas.Coordinates()
            .Where( n => antennas[n] != '.' )
            .Select( n => {
                // Now we need to find all matching nodes
                var c = antennas[n];
                return antennas.Coordinates()
                    .Where( m => m != n && antennas[m] == c )
                    .Select( m => Helpers.AntinodesFromPair( m, n ) )
                    .Flatten();
            } )
            .Flatten()
            .Where( antennas.Contains )
            .ToHashSet()
            .Count()
            ;
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var antennas = ProdAntennas;
        var result = antennas.Coordinates()
            .Where( n => antennas[n] != '.' )
            .Select( n => {
                // Now we need to find all matching nodes
                var c = antennas[n];
                return antennas.Coordinates()
                    .Where( m => m != n && antennas[m] == c )
                    .Select( m => Helpers.AntinodesFromPair2( m, n )
                        .TakeWhile( o => antennas.Contains( o ) )
                    )
                    .Flatten();
            } )
            .Flatten()
            .ToHashSet()
            .Count()
            ;
        Console.WriteLine( result );
    }
}

internal static partial class Helpers
{
    public static IEnumerable<Node2D<int>> AntinodesFromPair(
        Node2D<int> pos1,
        Node2D<int> pos2
    )
    {
        var diff = pos2 - pos1;
        // With an = pos2 + diff:
        // an - pos1 = pos2 + pos2 - pos1 - pos1 
        //   = 2*(pos2 - pos1) = 2*diff
        // While
        // an - pos2 = pos2 + diff - pos2 = diff
        yield return pos2 + diff;
    }

    public static IEnumerable<Node2D<int>> AntinodesFromPair2(
        Node2D<int> pos1,
        Node2D<int> pos2
    )
    {
        var diff = pos2 - pos1;
        return pos2.FixPoint( n => n + diff );
    }
}
