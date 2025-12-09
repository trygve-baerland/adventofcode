
using Sprache;
using AoC.Utils;
using AoC.Y2023;

namespace AoC.Y2025;

public sealed class Day9 : IPuzzle
{
    private IEnumerable<Node2D<long>> TestData =
        "2025/inputdata/day9_test.txt".GetLines()
        .Select( Helpers.RedTile.Parse ).ToList();
    private IEnumerable<Node2D<long>> Data =
        "2025/inputdata/day9.txt".GetLines()
        .Select( Helpers.RedTile.Parse ).ToList();

    public void Part1()
    {
        var data = Data;
        var result = data.Pairs()
            .Select( p => {
                var d = p.Item2 - p.Item1;
                return (System.Math.Abs( d.X ) + 1) * (System.Math.Abs( d.Y ) + 1);
            } )
            .Max();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var data = TestData;
        var result = data.Pairs()
            .Select( p => new RedCarpet( p.Item1, p.Item2 ) )
            .Where( r => data.TakeTwo()
                .All( edge => {
                    // At least one of the nodes are inside
                    if ( r.StrictContains( edge.Item1 ) || r.StrictContains( edge.Item2 ) )
                    {
                        return false;
                    }
                    // Both nodes are outside the interior of the box.
                    // Now, it can still be invalid, mind you.
                    // Both points must be on the same side of the box,
                    // in either x- or y-direction.
                    return r.Crosses( edge.Item1, edge.Item2 );
                } ) )
            .Select( r => {
                Console.WriteLine( $"{r.A}, {r.B}: {r.Area()}" );
                return r.Area();
            } )
            .Max();

        Console.WriteLine( result );
    }
}

record struct RedCarpet( Node2D<long> A, Node2D<long> B )
{
    public long Area()
    {
        var d = B - A;
        return (System.Math.Abs( d.X ) + 1) * (System.Math.Abs( d.Y ) + 1);
    }

    public bool Crosses( Node2D<long> a, Node2D<long> b )
    {
        var minX = long.Min( A.X, B.X );
        var minY = long.Min( A.Y, B.Y );
        return a.X <= minX && b.X <= minX ||
            a.Y <= minY && b.Y <= minY;
    }

    public bool StrictContains( Node2D<long> other )
    {
        return other.X > long.Min( A.X, B.X ) &&
            other.X < long.Max( A.X, B.X ) &&
            other.Y > long.Min( A.Y, B.Y ) &&
            other.Y < long.Max( A.Y, B.Y );
    }

    public bool Contains( Node2D<long> other )
    {
        return other.X >= long.Min( A.X, B.X ) &&
            other.X <= long.Max( A.X, B.X ) &&
            other.Y >= long.Min( A.Y, B.Y ) &&
            other.Y <= long.Max( A.Y, B.Y );
    }
}

static partial class Helpers
{
    public static readonly Parser<Node2D<long>> RedTile =
        Long.Then( n1 => Parse.Char( ',' ).Select( _ => n1 ) )
        .Then( n1 => Long.Select( n2 => (n1, n2) ) )
        .Select( pair => new Node2D<long>( pair.n1, pair.n2 ) );
}
