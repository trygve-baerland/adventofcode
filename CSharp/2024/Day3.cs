using AoC.Utils;
using System.Text.RegularExpressions;
namespace AoC.Y2024;

public sealed class Day3 : IPuzzle
{
    public string TestData = "2024/inputdata/day3_test.txt".GetText();
    public string Data = "2024/inputdata/day3.txt".GetText();
    public void Part1()
    {
        var result = RegexPart1.Matches( Data ).Select( Mul ).Sum();

        Console.WriteLine( result );
    }

    public void Part2()
    {
        bool enabled = true;
        long sum = 0;
        foreach ( Match match in RegexPart2.Matches( Data ) )
        {
            switch ( match.ToString() )
            {
                case "do()":
                    enabled = true;
                    break;
                case "don't()":
                    enabled = false;
                    break;
                default:
                    if ( enabled )
                    {
                        sum += Mul( match );
                    }
                    break;
            }
        }
        Console.WriteLine( sum );
    }

    private Regex RegexPart1 { get; } = new( @"mul\((?<n1>\d{1,3}),(?<n2>\d{1,3})\)" );
    private Regex RegexPart2 { get; } = new( @"mul\((?<n1>\d{1,3}),(?<n2>\d{1,3})\)|do\(\)|don't\(\)" );

    private static long Mul( Match match )
    {
        var l1 = long.Parse( match.Groups["n1"].Value );
        var l2 = long.Parse( match.Groups["n2"].Value );
        return l1 * l2;
    }
}
