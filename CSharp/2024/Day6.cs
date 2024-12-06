using AoC.Utils;

namespace AoC.Y2024;

public sealed class Day6 : IPuzzle
{

    private CharMap TestMap = CharMap.FromFile( "2024/inputdata/day6_test.txt" );
    private CharMap ProdMap = CharMap.FromFile( "2024/inputdata/day6.txt" );

    public void Part1()
    {
        var map = ProdMap;

        // Find starting position:
        var pos = map.Find( '^' );
        var state = new PlaceAndDir {
            Pos = pos,
            Dir = Tangent2D<int>.Up
        };

        var result = state.GoAlong( map ).Select( s => s.Pos ).ToHashSet().Count();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var map = ProdMap;

        // Find starting position:
        var pos = map.Find( '^' );
        var state = new PlaceAndDir {
            Pos = pos,
            Dir = Tangent2D<int>.Up
        };

        var visited = new HashSet<PlaceAndDir> {
            state
        };
        var candidates = new HashSet<Node2D<int>>();
        long result = 0;
        while ( state.MoveNext( map ) )
        {
            visited.Add( state );
            var right = state.TurnRight();
            var nextPos = state.Pos + state.Dir;
            // First, check if we can place an obstruction at the next point
            if ( !candidates.Contains( nextPos ) && map.Contains( nextPos ) && map[nextPos] != '#' )
            {
                // Next, we attempt to place a stone, and see if this leads to a cycle being created
                var tmpVisited = new HashSet<PlaceAndDir>();
                if ( right.GoAlong( map.With( '#', state.Pos + state.Dir ) )
                    .Any( s => {
                        if ( visited.Contains( s ) || tmpVisited.Contains( s ) ) return true;
                        tmpVisited.Add( s );
                        return false;
                    } )
                     )
                {
                    result += 1;
                }
            }
            candidates.Add( nextPos );
        }
        Console.WriteLine( result );
    }
}

internal struct PlaceAndDir
{
    public Node2D<int> Pos { get; set; }
    public Tangent2D<int> Dir { get; set; }

    public bool MoveNext( CharMap map )
    {
        var next = Pos + Dir;
        if ( !map.Contains( next ) )
        {
            return false;
        }

        if ( map[next] != '#' )
        {
            Pos = next;
        }
        else
        {
            Dir = Dir.TurnRight();
        }
        return true;
    }

    public IEnumerable<PlaceAndDir> GoAlong( CharMap map )
    {
        var current = this;
        yield return current;
        while ( current.MoveNext( map ) )
        {
            yield return current;
        }
    }

    public PlaceAndDir TurnRight()
    {
        return new PlaceAndDir {
            Pos = Pos,
            Dir = Dir.TurnRight()
        };
    }

    public PlaceAndDir AddDir() => new PlaceAndDir {
        Pos = Pos + Dir,
        Dir = Dir
    };

    public IEnumerable<PlaceAndDir> Ray()
    {
        var current = this;
        while ( true )
        {
            yield return current;
            current = current.AddDir();
        }
    }
}
