using System.Text;
using AoC.Utils;
using Sprache;

namespace AoC.Y2025;

public sealed class Day12 : IPuzzle
{
    private AllPresentSpaces TestData =
        Helpers.AllPresentSpaces.Parse( "2025/inputdata/day12_test.txt".GetText() );
    private AllPresentSpaces Data =
        Helpers.AllPresentSpaces.Parse( "2025/inputdata/day12.txt".GetText() );

    public void Part1()
    {
        var data = Data;
        Console.WriteLine( data );
        var result = data.Regions
            .Count( r => Helpers.CanFit( data.Shapes, r ) );
        Console.WriteLine( result );
    }

    public void Part2() { }
}

record struct PresentShape( char[][] Shape )
{
    public override readonly string ToString()
    {
        return string.Concat( Shape.Select( row => $"{string.Concat( row )}\n" ) );
    }

    public int Size => Shape.Sum( row => row.Count( c => c == '#' ) );
}

record struct PresentSpace( int Height, int Width, List<long> Counts )
{
    public override readonly string ToString()
    {
        return $"{Height}x{Width}: {string.Join( ' ', Counts )}";
    }
}

record struct AllPresentSpaces( Dictionary<int, PresentShape> Shapes, List<PresentSpace> Regions )
{
    public override readonly string ToString()
    {
        var sb = new StringBuilder();
        foreach ( var item in Shapes )
        {
            sb.AppendLine( $"{item.Key}:" );
            sb.AppendLine( $"{item.Value}\n" );
        }
        foreach ( var region in Regions )
        {
            sb.AppendLine( region.ToString() );
        }
        return sb.ToString();
    }
}


static partial class Helpers
{
    public static bool CanFit( Dictionary<int, PresentShape> shapes, PresentSpace region )
    {
        var presentArea = region.Counts.Select( ( count, i ) => count * shapes[i].Size ).Sum();
        var totalArea = region.Height * region.Width;
        Console.WriteLine( $"{presentArea} and {totalArea}" );
        return totalArea >= presentArea;
    }
    public static readonly Parser<PresentShape> PresentShape =
        Parse.Char( '.' ).Or( Parse.Char( '#' ) ).Repeat( 3 )
        .Select( c => c.ToArray() )
        .Then( row => Parse.LineEnd.Select( _ => row ) )
        .Repeat( 3 )
        .Select( rows => new PresentShape( rows.ToArray() ) );

    public static readonly Parser<Dictionary<int, PresentShape>> AllShapes =
        Parse.Number.Select( int.Parse )
        .Then( n => Parse.String( ":\n" ).Select( _ => n ) )
        .Then( n => PresentShape.Select( shape => (n, shape) ) )
        .DelimitedBy( Parse.LineEnd.AtLeastOnce() )
        .Select( items => items.ToDictionary( item => item.n, item => item.shape ) );

    public static readonly Parser<PresentSpace> PresentSpace =
        Parse.Number.Select( int.Parse )
        .Then( h => Parse.Char( 'x' ).Select( _ => h ) )
        .Then( h => Parse.Number.Select( int.Parse ).Select( w => (h, w) ) )
        .Then( p => Parse.String( ": " ).Select( _ => p ) )
        .Then( p => Long.DelimitedBy( Parse.Char( ' ' ) ).Select( counts => (p.h, p.w, counts) ) )
        .Select( p => new PresentSpace( p.h, p.w, p.counts.ToList() ) );

    public static readonly Parser<AllPresentSpaces> AllPresentSpaces =
        AllShapes.Then( shapes => Parse.LineEnd.Many().Select( _ => shapes ) )
        .Then( shapes => PresentSpace.DelimitedBy( Parse.LineEnd ).Select( spaces => (shapes, spaces) ) )
        .Select( p => new Y2025.AllPresentSpaces( p.shapes, p.spaces.ToList() ) );
}
