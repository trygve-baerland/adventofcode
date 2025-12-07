
using AoC.Utils;

namespace AoC.Y2025;

public sealed class Day7 : IPuzzle
{
    private CharMap TestData =
        CharMap.FromFile( "2025/inputdata/day7_test.txt" );
    private CharMap Data =
        CharMap.FromFile( "2025/inputdata/day7.txt" );

    public void Part1()
    {
        var data = Data;
        var source = data.Find( 'S' );
        var result = Graph.BFS(
            source,
            node => {
                //Console.WriteLine( $"Visiting {node}" );
                return Helpers.GetAdjacentTachyons( data, node );
            }
        ).Where( pair => data[pair.node] == '^' ).Count();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var data = Data;
        var memMap = data.Select( _ => 0L );
        // We know that at the bottom row, there are only
        // 1 path
        foreach ( var node in Enumerable.Range( 0, data.Width )
            .Select( j => new Node2D<int>( data.Height - 1, j ) )
        )
        {
            memMap[node] = 1;
        }
        // Updata memMap bottom-up
        data.AllRows().Reverse().Skip( 1 )
            .ForEach( row => {
                row.ForEach( node => {
                    switch ( data[node] )
                    {
                        case '^':
                            memMap[node] =
                                memMap[node.Left().Down()] + memMap[node.Right().Down()];
                            break;
                        default:
                            memMap[node] = memMap[node.Down()];
                            break;
                    }
                } );
            } );
        var source = data.Find( 'S' );
        Console.WriteLine( memMap[source] );
    }
}

static partial class Helpers
{
    public static IEnumerable<Node2D<int>> GetAdjacentTachyons( CharMap map, Node2D<int> node )
    {
        switch ( map[node] )
        {
            case '^':
                // Emit left and right
                var left = node.Left();
                if ( map.Contains( left ) ) yield return left;
                var right = node.Right();
                if ( map.Contains( right ) ) yield return right;
                break;
            default:
                // only emit down
                var down = node.Down();
                if ( map.Contains( down ) ) yield return down;
                break;
        }
    }
}
