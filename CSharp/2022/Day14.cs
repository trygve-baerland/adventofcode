using AoC.Utils;
using Sprache;

namespace AoC.Y2022;

public sealed class Day14 : IPuzzle
{
    private IEnumerable<IEnumerable<Node2D<int>>> TestRocks() =>
        "2022/inputdata/day14_test.txt".GetLines()
        .Select( Day14Parser.GetRockLine )
        .Select( rockLine => rockLine.ToList() )
        .ToList();

    private IEnumerable<IEnumerable<Node2D<int>>> ActualRocks() =>
        "2022/inputdata/day14.txt".GetLines()
        .Select( Day14Parser.GetRockLine )
        .Select( rockLine => rockLine.ToList() )
        .ToList();

    public void Part1()
    {
        var cave = new Cave( ActualRocks(), withFloor: false );
        var source = new Node2D<int>( 500, 0 );
        int counter = 0;
        while ( cave.DropSand( source ) != BlockedEnum.Abyss )
        {
            counter++;
        }
        Console.WriteLine( counter );
    }

    public void Part2()
    {
        var cave = new Cave( ActualRocks(), withFloor: true );
        var source = new Node2D<int>( 500, 0 );

        var result = Graph.DFS(
            source: source,
            adjacentNodes: node => cave.GetNextNodes( node )
        ).Count();

        Console.WriteLine( result );
    }
}
public enum BlockedEnum
{
    Blocked,
    Free,
    Abyss,
}

public class Cave
{
    private char[][] Array { get; set; }
    private Node2D<int> TopLeft { get; set; }
    private Node2D<int> BottomRight { get; set; }
    public HashSet<Node2D<int>> Blockers { get; } = new();
    private bool WithFloor { get; init; }
    public Cave( IEnumerable<IEnumerable<Node2D<int>>> rockLines, bool withFloor = false )
    {
        // Get bounding box based on rock lines:
        WithFloor = withFloor;
        int minX = rockLines.Select(
            rockLine => rockLine.Select( node => node.X ).Min()
        ).Min();
        int maxX = rockLines.Select(
            rockLine => rockLine.Select( node => node.X ).Max()
        ).Max();
        int minY = rockLines.Select(
            rockLine => rockLine.Select( node => node.Y ).Min()
        ).Min();
        minY = System.Math.Min( 0, minY );
        int maxY = rockLines.Select(
            rockLine => rockLine.Select( node => node.Y ).Max()
        ).Max();
        if ( withFloor )
        {
            maxY += 2;
        }
        TopLeft = new Node2D<int> { X = minX, Y = minY };
        BottomRight = new Node2D<int> { X = maxX, Y = maxY };
        // Initialize array:
        Array = Enumerable.Range( 0, maxY - minY + 1 )
            .Select(
                _ => Enumerable.Range( 0, maxX - minX + 1 )
                    .Select( _ => '.' ).ToArray()
            )
            .ToArray();
        // Draw in rocks:
        rockLines.ForEach( rockLine => { DrawRockLine( rockLine ); } );
        // Draw floor:
        if ( withFloor )
        {
            DrawRockLine( new List<Node2D<int>>() {
                new Node2D<int> {
                    X = minX, Y = maxY
                },
                BottomRight
            } );
        }
    }
    public void DrawRockLine( IEnumerable<Node2D<int>> rockLine )
    {
        var enumerator = rockLine.GetEnumerator();
        if ( !enumerator.MoveNext() )
        {
            return;
        }
        var prev = enumerator.Current;
        Node2D<int> next;
        while ( enumerator.MoveNext() )
        {
            next = enumerator.Current;
            foreach ( var node in prev.NodesTo( next, true ) )
            {
                Blockers.Add( node );
                ColorNode( node, '#' );
            }
            prev = next;
        }
    }

    public void Print()
    {
        foreach ( var row in Array )
        {
            Console.WriteLine( new string( row ) );
        }
        Console.WriteLine( $"Bounding box: {TopLeft} <-> {BottomRight}" );
    }

    public Tangent2D<int>? GetLocalCoords( Node2D<int> node )
    {
        if ( !(GreaterThan( node, TopLeft ) && LessThan( node, BottomRight )) )
        {
            return null;
        }
        return node - TopLeft;
    }

    public static bool LessThan( Node2D<int> lhs, Node2D<int> rhs ) =>
        lhs.X <= rhs.X && lhs.Y <= rhs.Y;

    public static bool GreaterThan( Node2D<int> lhs, Node2D<int> rhs ) =>
        lhs.X >= rhs.X && lhs.Y >= rhs.Y;

    public void ColorNode( Node2D<int> node, char c )
    {
        if ( GetLocalCoords( node ) is Tangent2D<int> t )
        {
            Array[t.Y][t.X] = c;
        }
    }


    public BlockedEnum DropSand( Node2D<int> start )
    {
        var sandPos = start;
        BlockedEnum blocked = BlockedEnum.Free;
        while ( blocked == BlockedEnum.Free )
        {
            (sandPos, blocked) = DropDirections // Go through each direction:
                .Select( dir => sandPos + dir )
                .Select( pos => (pos, NotBlocked( pos )) ) // update new position
                .FirstOrDefault(
                    item => item.Item2 != BlockedEnum.Blocked,
                    (sandPos, BlockedEnum.Blocked) );
        }
        if ( sandPos == start && NotBlocked( sandPos ) == BlockedEnum.Blocked )
        {
            return BlockedEnum.Abyss;
        }
        if ( blocked == BlockedEnum.Blocked )
        {
            // Draw sand on grid:
            ColorNode( sandPos, 'o' );
            Blockers.Add( sandPos );
        }
        return blocked;
    }

    public BlockedEnum NotBlocked( Node2D<int> node )
    {
        if ( Blockers.Contains( node ) )
        {
            return BlockedEnum.Blocked;
        }
        else if ( node.Y >= BottomRight.Y )
        {
            if ( WithFloor ) return BlockedEnum.Blocked;
            else return BlockedEnum.Abyss;
        }
        return BlockedEnum.Free;
    }

    public static readonly List<(int x, int y)> DropDirections = new() { (0, 1), (-1, 1), (1, 1) };

    public IEnumerable<Node2D<int>> GetNextNodes( Node2D<int> node ) =>
        DropDirections
        .Select( dir => node + dir )
        .Where( v => NotBlocked( v ) == BlockedEnum.Free );

}
public static class Day14Parser
{
    public static readonly Parser<int> UInt =
        from number in Parse.Number
        select int.Parse( number );

    public static readonly Parser<Node2D<int>> IntPair =
        UInt
        .Then( num1 => Parse.Char( ',' ).Select( _ => num1 ) )
        .Then( num1 => UInt.Select( num2 => new Node2D<int>( num1, num2 ) ) );

    public static readonly Parser<IEnumerable<Node2D<int>>> RockLine =
        IntPair.DelimitedBy( Parse.String( " -> " ) );

    // Helper methods:
    public static IEnumerable<Node2D<int>> GetRockLine( string text )
    {
        return RockLine.Parse( text );
    }
}
