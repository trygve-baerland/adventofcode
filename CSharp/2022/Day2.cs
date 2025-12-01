using AoC.Utils;

namespace AoC.Y2022;

public sealed class Day2 : IPuzzle
{
    public List<(int lhs, int rhs)> TestLines() => "2022/inputdata/day2_test.txt".GetLines().Select( Day2Helpers.ParseLine ).ToList();
    public List<(int lhs, int rhs)> ActualLines() => "2022/inputdata/day2.txt".GetLines().Select( Day2Helpers.ParseLine ).ToList();
    public void Part1()
    {
        var result = ActualLines();
        Day2Helpers.PrintResults( result );
    }

    public void Part2()
    {
        var result = ActualLines().Select( item => (lhs: item.lhs, rhs: Day2Helpers.GetCorrectPlay( item.lhs, item.rhs )) );
        Day2Helpers.PrintResults( result );
    }
}
internal static class Day2Helpers
{
    public static void PrintResults( IEnumerable<(int lhs, int rhs)> games )
    {
        var playSum = games.Select( item => ScorePlay( item.rhs ) ).Sum();
        var gameSum = games.Select( item => ScoreGame( item.lhs, item.rhs ) ).Sum();
        Console.WriteLine( $"Play sum: {playSum}" );
        Console.WriteLine( $"Game sum: {gameSum}" );
        Console.WriteLine( $"Total: {playSum + gameSum}" );
    }
    public static (int lhs, int rhs) ParseLine( string line )
    {
        var items = line.Split( " " );
        var lhs = items[0] switch {
            "A" => 0, // Rock
            "B" => 1, // Paper
            "C" => 2, // Scissors
            _ => throw new System.Exception( $"Invalid play: {items[0]}" )
        };
        var rhs = items[1] switch {
            "X" => 0,
            "Y" => 1,
            "Z" => 2,
            _ => throw new System.Exception( $"Invalid play: {items[1]}" )
        };
        return (lhs, rhs);
    }

    public static int ScorePlay( int play )
    {
        return play + 1;
    }

    public static int ScoreGame( int lhs, int rhs )
    {
        // Rock beats scissors,
        // Paper beats scissors,
        // Scissors beats paper

        // Draws (those are easiest)
        if ( lhs == rhs ) return 3;
        // Check if rhs (us) win:
        return 6 * Convert.ToInt32( rhs == (lhs + 1) % 3 ); // mext element in cyclical arrangement
    }

    public static int GetCorrectPlay( int lhs, int rhs )
    {
        return (lhs + (rhs - 1) + 3) % 3;
    }
}
