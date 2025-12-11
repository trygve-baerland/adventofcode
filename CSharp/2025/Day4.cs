
using AoC.Utils;

namespace AoC.Y2025;

public sealed class Day4 : IPuzzle
{
    private CharMap TestData = CharMap.FromFile( "2025/inputdata/day4_test.txt" );
    private CharMap Data = CharMap.FromFile( "2025/inputdata/day4.txt" );

    public void Part1()
    {
        var map = Data;
        var result = map.Coordinates()
            .Count( x => map[x] == '@' && x.NeighboursWithDiagonals()
                .Count( y => map.Contains( y ) && map[y] == '@' ) < 4
            );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = Data;
        var removed = new HashSet<Node2D<int>>();
        while ( true )
        {
            var cands = map.Coordinates()
                .Where( x => map[x] == '@' && x.NeighboursWithDiagonals()
                    .Count( y => map.Contains( y ) && map[y] == '@' && !removed.Contains( y ) ) < 4
                )
                .ToList();
            if ( cands.All( x => removed.Contains( x ) ) )
            {
                break;
            }
            cands.ForEach( x => removed.Add( x ) );

        }
        Console.WriteLine( removed.Count() );
    }
}
