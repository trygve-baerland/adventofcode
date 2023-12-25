using System.Text.RegularExpressions;
using AoC.Utils;

namespace AoC.Y2023;

public sealed class Day1 : IPuzzle
{
    public IEnumerable<string> TestData => "2023/inputdata/day1_test.txt".GetLines();
    public IEnumerable<string> ActualData => "2023/inputdata/day1.txt".GetLines();

    public void Part1()
    {
        var result = ActualData
          .Select( l => GetNumber( l, RegexPart1 ) )
          .Sum();
        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var result = ActualData
          .Select( l => GetNumber( l, RegexPart2 ) )
          .Sum();
        Console.WriteLine( $"Result: {result}" );
    }

    private Regex RegexPart1 { get; } = new( @"(?<number>\d)" );
    private Regex RegexPart2 { get; } = new( @"(?=(?<number>\d|one|two|three|four|five|six|seven|eight|nine))" );

    private int GetNumber( string line, Regex regex )
    {
        var matches = regex.Matches( line );
        return 10 * ConvertDigit( Digit( matches.First() ) ) + ConvertDigit( Digit( matches.Last() ) );
    }

    private static string Digit( Match match ) => match.Groups["number"].Value;

    private int ConvertDigit( string number )
    {
        try { return int.Parse( number ); }
        catch ( Exception )
        {
            return number switch {
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                _ => throw new ArgumentException( $"Invalid number {number}" )
            };
        }
    }
}
