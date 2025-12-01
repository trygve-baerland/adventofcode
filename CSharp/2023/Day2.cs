using AoC.Utils;
using Sprache;

namespace AoC.Y2023;

public sealed class Day2 : IPuzzle
{
    public IEnumerable<Game> TestGames =
        "2023/inputdata/day2_test.txt".GetLines().Select( Game.FromString ).ToList();

    public IEnumerable<Game> ActualGames =
        "2023/inputdata/day2.txt".GetLines().Select( Game.FromString ).ToList();

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
static partial class Helpers
{
    #region parsing stuff
    private static readonly Parser<int> Number = Parse.Number.Select( int.Parse );

    private static Parser<(string color, int value)> Color( string colorName ) =>
        Number.Then( number => Parse.Char( ' ' ).Select( _ => number ) )
            .Then( number => Parse.String( colorName ).Select( color => (String.Concat( color ), number) ) )
            .Select( item => item );

    private static Parser<RGB> Set =
        Color( "red" ).Or( Color( "green" ) ).Or( Color( "blue" ) ).DelimitedBy( Parse.Char( ',' ).Token() )
        .Select( colors => colors.ToDictionary() )
        .Select( valDict => new RGB(
            red: valDict.GetValueOrDefault( "red", defaultValue: 0 ),
            green: valDict.GetValueOrDefault( "green", defaultValue: 0 ),
            blue: valDict.GetValueOrDefault( "blue", defaultValue: 0 )
            )
        );

    public static Parser<Game> Game =
        Parse.String( "Game " )
        .Then( _ => Number )
        .Then( number => Parse.Char( ':' ).Token().Select( _ => number ) )
        .Then( id => Set.DelimitedBy( Parse.Char( ';' ).Token() )
            .Select( sets => new Game( id, sets.ToList() ) )
        );

    #endregion parsing stuff
}
