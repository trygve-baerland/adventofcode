using AoC.Utils;
namespace AoC.Y2022;

public sealed class Day9 : IPuzzle
{
    private IEnumerable<RopeMove> TestMoves() =>
        "2022/inputdata/day9_test.txt".GetLines().Select( RopeMove.Parse );

    private IEnumerable<RopeMove> ActualMoves() =>
        "2022/inputdata/day9.txt".GetLines().Select( RopeMove.Parse );
    public void Part1()
    {
        // Initialize stuff:
        var moves = ActualMoves();
        Rope rope = new( 1 );
        rope.Visited.Add( (0, 0) );
        // Go through each line:
        foreach ( var move in moves )
        {
            // Get direction and number of times:
            Enumerable.Range( 0, move.Distance ).ForEach( arg => rope.Move( move.Direction ) );
        }

        // Part 1:
        Console.WriteLine( $"{rope.Visited.Count}" );
    }

    public void Part2()
    {
        var moves = ActualMoves();
        Rope rope = new( 9 );
        rope.Visited.Add( (0, 0) );
        // Go through each line:
        foreach ( var move in moves )
        {
            // Repeat moves a number of times:
            Enumerable.Range( 0, move.Distance )
            .ForEach( arg => rope.Move( move.Direction ) );
        }

        Console.WriteLine( rope.Visited.Count );
    }

}

public enum Directions
{
    R,
    L,
    U,
    D
}

internal record struct RopeMove( Directions Direction, int Distance )
{
    public static RopeMove Parse( string s )
    {
        var items = s.Split( " " );
        if ( !Enum.TryParse( items[0], out Directions dir ) )
        {
            throw new Exception( $"Unable to parse direction {items[0]}" );
        }
        ;
        return new( dir, int.Parse( items[1] ) );
    }
}

public class Rope
{
    public Node2D<int> Head { get; set; } = new( 0, 0 );

    public List<Node2D<int>> Tail { get; private set; }

    public Rope( int tailLength )
    {
        if ( tailLength < 1 )
        {
            throw new Exception( "Tail must be at least 1 knot long" );
        }

        // Create a new (distinct) Node element for each knot:
        Tail = Enumerable.Range( 0, tailLength ).Select( item => new Node2D<int>( 0, 0 ) ).ToList();
    }
    public HashSet<(int, int)> Visited { get; } = new();

    public void Move( Directions dir )
    {
        // Update head position:
        var dx = dir switch {
            Directions.R => (0, 1),
            Directions.L => (0, -1),
            Directions.U => (1, 0),
            Directions.D => (-1, 0),
            _ => throw new Exception( $"Unknown direction {dir}" )
        };
        Head += dx;
        // Update tail positions:
        Tail = Tail.Accumulate(
            seed: Head,
            func: ( prev, knot ) => {
                knot = knot.MoveTowards( prev );
                return knot;
            }
        )
        .ToList();
        Visited.Add( Tail.Last().ToTangent() );
    }
}

internal static class Day9Helpers
{
    public static Node2D<int> MoveTowards( this Node2D<int> node, Node2D<int> head )
    {
        var diff = head - node;
        while ( diff.LInf() > 1 )
        {
            // We crop to moving either -1, 0, or 1 in either direction.
            node += (System.Math.Sign( diff.X ), System.Math.Sign( diff.Y ));
            diff = head - node;
        }
        return node;
    }
}
