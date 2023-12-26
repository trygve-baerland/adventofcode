using Sprache;
using AoC.Utils;
namespace AoC.Y2023;

public sealed class Day18 : IPuzzle
{
    public IEnumerable<(TrenchWall naive, TrenchWall hex)> TestWalls() => "2023/inputdata/day18_test.txt".GetLines().Select( line => Helpers.TrenchWallParser.Parse( line ) );
    public IEnumerable<(TrenchWall naive, TrenchWall hex)> ActualWalls() => "2023/inputdata/day18.txt".GetLines().Select( line => Helpers.TrenchWallParser.Parse( line ) );
    public void Part1()
    {
        var start = new Node2D<long>( 0, 0 );
        var (end, area, circumference) = ActualWalls().Select( item => item.naive )
        .Aggregate(
            seed: (start, area: 0L, circumference: 0L),
            func: ( state, wall ) => {
                var end = wall.StartAt( state.start );
                var area = state.area + state.start.Cross( end );
                var circumference = state.circumference + wall.Length;
                return (end, area, circumference);
            }
        );
        var result = (System.Math.Abs( area ) + circumference) / 2 + 1;
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var start = new Node2D<long>( 0, 0 );
        var (end, area, circumference) = ActualWalls().Select( item => item.hex )
        .Aggregate(
            seed: (start, area: 0L, circumference: 0L),
            func: ( state, wall ) => {
                var end = wall.StartAt( state.start );
                var area = state.area + state.start.Cross( end );
                var circumference = state.circumference + wall.Length;
                return (end, area, circumference);
            }
        );
        var result = (System.Math.Abs( area ) + circumference) / 2 + 1;
        Console.WriteLine( result );
    }
}

public record struct TrenchWall( Tangent2D<int> Direction, int Length )
{
    public override string ToString()
    {
        return $"{Helpers.DirectionToChar( Direction )} {Length}";
    }

    public Node2D<long> StartAt( Node2D<long> start ) =>
        new Node2D<long>( start.X + Length * Direction.X, start.Y + Length * Direction.Y );
    public static TrenchWall FromHex( string hex )
    {
        var dir = Helpers.CharToDirection( hex[^1] switch {
            '0' => 'R',
            '1' => 'D',
            '2' => 'L',
            '3' => 'U',
            _ => throw new Exception( $"Invalid hex direction {hex[^1]}" )
        } );
        var length = hex[..^1].Aggregate( 0, ( acc, d ) => acc * 16 + Helpers.HexDigit( d ) );
        return new TrenchWall( dir, length );
    }
}

public static partial class Helpers
{
    #region parsers
    public static readonly Parser<(int x, int y)> TrenchDirection =
        Parse.Chars( 'U', 'D', 'L', 'R' ).Select( CharToDirection );

    public static readonly Parser<int> TrenchLength =
        Parse.Number.Select( int.Parse );

    public static readonly Parser<TrenchWall> HexTrenchParser =
        Parse.String( "(#" )
            .Then( _ => Parse.LetterOrDigit.Repeat( 6 ).Text() )
            .Then( s => Parse.String( ")" ).Select( _ => s ) )
            .Select( TrenchWall.FromHex );

    public static readonly Parser<(TrenchWall naive, TrenchWall hex)> TrenchWallParser =
        TrenchDirection
            .Then( dir => Parse.Char( ' ' ).AtLeastOnce().Select( _ => dir ) )
            .Then( dir => TrenchLength.Select( length => (dir, length) ) )
            .Then( dirLength => Parse.Char( ' ' ).AtLeastOnce().Select( _ => dirLength ) )
            .Then( dirLength => HexTrenchParser.Select( wall => (dirLength.dir, dirLength.length, wall) ) )
            .Select( tuple => (new TrenchWall( tuple.dir, tuple.length ), tuple.wall) );
    #endregion parsers

    public static (int x, int y) CharToDirection( char c ) => c switch {
        'R' => (0, 1),
        'D' => (1, 0),
        'L' => (0, -1),
        'U' => (-1, 0),
        _ => throw new ArgumentException( $"Invalid direction: {c}" )
    };

    public static char DirectionToChar( (int x, int y) dir ) => dir switch {
        (0, 1 ) => 'R',
        (1, 0 ) => 'D',
        (0, -1 ) => 'L',
        (-1, 0 ) => 'U',
        _ => throw new ArgumentException( $"Invalid direction: {dir}" )
    };

    public static int HexDigit( char c ) => c switch {
        >= '0' and <= '9' => c - '0',
        >= 'a' and <= 'f' => c - 'a' + 10,
        _ => throw new ArgumentException( $"Invalid hex digit: {c}" )
    };
}
