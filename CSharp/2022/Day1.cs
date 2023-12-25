using Sprache;
using AoC.Utils;

namespace AoC.Y2022;

public sealed class Day1 : IPuzzle
{
    List<int> TestElves() => Helpers.CaloriesPerElf.Parse( string.Concat( "2022/inputdata/day1_test.txt".Stream().GetChars() ) ).ToList();
    List<int> ActualElves() => Helpers.CaloriesPerElf.Parse( string.Concat( "2022/inputdata/day1.txt".Stream().GetChars() ) ).ToList();
    public void Part1()
    {
        var elves = ActualElves();
        Console.WriteLine( $"The elf with the most hoarded calories: {elves.Max()}" );
    }

    public void Part2()
    {
        var result = ActualElves().OrderByDescending( cal => cal ).Take( 3 ).Sum();
        Console.WriteLine( $"The three richest elves have a total of {result} calories." );
    }
}
public static partial class Helpers
{

    public static readonly Parser<IEnumerable<int>> CaloriesForOneElf =
        Parse.Number.Select( int.Parse ).DelimitedBy( Parse.LineEnd.Once() );

    public static readonly Parser<IEnumerable<int>> CaloriesPerElf =
        CaloriesForOneElf.Select( cals => cals.Sum() )
        .DelimitedBy( Parse.Char( '\n' ).AtLeastOnce() );
}
