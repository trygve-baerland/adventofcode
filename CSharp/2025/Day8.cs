
using AoC.Utils;
using AoC.Y2023;
using Sprache;

namespace AoC.Y2025;

public sealed class Day8 : IPuzzle
{
    private IEnumerable<Node3D<long>> TestData =
        "2025/inputdata/day8_test.txt".GetLines().Select( Helpers.JunctionBox.Parse ).ToList();

    private IEnumerable<Node3D<long>> Data =
        "2025/inputdata/day8.txt".GetLines().Select( Helpers.JunctionBox.Parse ).ToList();

    public void Part1()
    {
        var data = Data;
        var repeats = 1000;


        List<Circuit> circuits = data.Select( n => new Circuit( [n] ) ).ToList();
        var pairs = data.Pairs().ToList().OrderBy( p => (p.Item1 - p.Item2).L2() );

        foreach ( var pair in pairs.Take( repeats ) )
        {
            //if ( circuits.Any( c => c.Contains( pair ) ) ) continue;

            var newCircuit = circuits.Where( c => c.ConnectedTo( pair ) )
                .Aggregate( Circuit.Empty(), ( res, c ) => res.Union( c ) );
            circuits = circuits.Where( c => !c.ConnectedTo( pair ) ).ToList();
            circuits.Add( newCircuit );
        }

        var result = circuits.OrderBy( c => -c.Count() ).Take( 3 )
            .Aggregate( 1L, ( res, c ) => res * c.Count() );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var data = Data;

        List<Circuit> circuits = data.Select( n => new Circuit( [n] ) ).ToList();
        var pairs = data.Pairs().ToList().OrderBy( p => (p.Item1 - p.Item2).L2() );

        foreach ( var pair in pairs )
        {
            var newCircuit = circuits.Where( c => c.ConnectedTo( pair ) )
                .Aggregate( Circuit.Empty(), ( res, c ) => res.Union( c ) );
            circuits = circuits.Where( c => !c.ConnectedTo( pair ) ).ToList();
            circuits.Add( newCircuit );
            if ( circuits.Count() == 1 )
            {
                var result = pair.Item1.X * pair.Item2.X;
                Console.WriteLine( result );
                return;
            }
        }
    }
}

record struct Circuit( HashSet<Node3D<long>> Boxes )
{
    public static Circuit Empty() => new Circuit( [] );

    public override readonly string ToString() => $"{{{string.Join( ',', Boxes )}}}";

    public long DistanceTo( Node3D<long> node ) => Boxes.Select( b => (b - node).L2() ).Min();
    public long DistanceTo( Circuit other ) => Boxes.Select( b => other.DistanceTo( b ) ).Min();

    public void Add( Node3D<long> node )
    {
        Boxes.Add( node );
    }
    public bool Contains( Node3D<long> node ) => Boxes.Contains( node );
    public bool Contains( (Node3D<long> n1, Node3D<long> n2) pair ) => Contains( pair.n1 ) && Contains( pair.n2 );

    public bool ConnectedTo( (Node3D<long> n1, Node3D<long> n2) pair ) => Contains( pair.n1 ) || Contains( pair.n2 );

    public bool Intersects( Circuit other ) => Boxes.Any( b => other.Contains( b ) );

    public Circuit Union( Circuit other ) => new Circuit( Boxes.Union( other.Boxes ).ToHashSet() );

    public int Count() => Boxes.Count();


}

static partial class Helpers
{
    public static readonly Parser<Node3D<long>> JunctionBox =
        Long.Then( n1 => Parse.Char( ',' ).Select( _ => n1 ) )
        .Then( n1 => Long.Then( n2 => Parse.Char( ',' ).Select( _ => (n1, n2) ) ) )
        .Then( pair => Long.Select( n3 => (pair.n1, pair.n2, n3) ) )
        .Select( triple => new Node3D<long>( triple.n1, triple.n2, triple.n3 ) );
}
