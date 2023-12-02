using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sprache;
using Utils;

namespace AoC;

public sealed class Day2 : IPuzzle
{
    public IEnumerable<Game> TestGames =
        "inputdata/day2_test.txt".GetLines().Select( Game.FromString ).ToList();

    public IEnumerable<Game> ActualGames =
        "inputdata/day2.txt".GetLines().Select( Game.FromString ).ToList();

    public RGB Threshold { get; } = new RGB( 12, 13, 14 );
    public void Part1()
    {
        var result = ActualGames.Where( game => game.Sets.All( set => set < Threshold ) )
            .Select( game => game.Id )
            .Sum();

        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var result = ActualGames.Select( game => game.MinViable().Power() ).Sum();
        Console.WriteLine( $"Result: {result}" );
    }
}

public struct RGB( int red, int green, int blue )
{
    public int Red { get; } = red;
    public int Green { get; } = green;
    public int Blue { get; } = blue;

    public override string ToString() => $"RGB({Red},{Green},{Blue})";

    public static bool operator <( RGB lhs, RGB rhs ) => lhs.Red <= rhs.Red && lhs.Green <= rhs.Green && lhs.Blue <= rhs.Blue;

    public static bool operator >( RGB lhs, RGB rhs ) => rhs < lhs;

    public int Power() => Red * Green * Blue;
}

public class Game( int id, List<RGB> sets )
{
    public int Id { get; } = id;
    public List<RGB> Sets { get; } = sets;

    public static Game FromString( string input ) => Helpers.Game.Parse( input );

    public override string ToString() => $"Game({Id}, {String.Join( ',', Sets )})";

    public RGB MinViable() => new RGB(
        Sets.Select( item => item.Red ).Max(),
        Sets.Select( item => item.Green ).Max(),
        Sets.Select( item => item.Blue ).Max()
    );
}
static class Helpers
{
    #region parsing stuff
    private static readonly Parser<int> Number =
        from number in Parse.Number
        select int.Parse( number );

    private static Parser<(string color, int value)> Color( string colorName ) =>
        from number in Number
        from _ in Parse.Char( ' ' )
        from color in Parse.String( colorName )
        select (String.Concat( color ), number);

    private static Parser<RGB> Set =
        from valDict in (from colors in Color( "red" )
            .Or( Color( "green" ) )
            .Or( Color( "blue" ) )
            .DelimitedBy( Parse.Char( ',' ).Token() )
                         select colors.ToDictionary())
        select new RGB(
            red: valDict.GetValueOrDefault( "red", defaultValue: 0 ),
            green: valDict.GetValueOrDefault( "green", defaultValue: 0 ),
            blue: valDict.GetValueOrDefault( "blue", defaultValue: 0 )
        );

    public static Parser<Game> Game =
        from leading in Parse.String( "Game " )
        from id in Number
        from _ in Parse.Char( ':' ).Token()
        from sets in Set.DelimitedBy( Parse.Char( ';' ).Token() )
        select new Game(
            id: id,
            sets: sets.ToList()
        );


    #endregion parsing stuff
}
