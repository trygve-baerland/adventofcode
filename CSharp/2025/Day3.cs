
using AoC.Utils;
using Sprache;

namespace AoC.Y2025;

public sealed class Day3 : IPuzzle
{
    private IEnumerable<BatteryBank> TestData =
        "2025/inputdata/day3_test.txt".GetLines().Select( Helpers.Battery.Parse );

    private IEnumerable<BatteryBank> Data =
        "2025/inputdata/day3.txt".GetLines().Select( Helpers.Battery.Parse ).ToList();

    public void Part1()
    {
        Console.WriteLine( Data.Select( b => b.Joltage( 2 ) ).Sum() );
    }

    public void Part2()
    {
        Console.WriteLine( Data.Select( b => b.Joltage( 12 ) ).Sum() );
    }
}

public record struct BatteryBank( List<int> Batteries )
{
    public readonly override string ToString()
    {
        return $"Battery[{string.Join( ',', Batteries )}]";
    }

    public long Joltage( int numDigits )
    {
        var remDigs = numDigits;
        long result = 0;
        List<int> cands = Batteries;
        while ( remDigs > 0 )
        {
            var maxVal = cands[..^(remDigs - 1)].Max();
            var maxI = cands[..^(remDigs - 1)].IndexOf( maxVal );
            // update stuff
            remDigs -= 1;
            result = 10 * result + maxVal;
            cands = cands[(maxI + 1)..];

        }
        return result;
    }
}

static partial class Helpers
{
    public static readonly Parser<BatteryBank> Battery =
        Parse.Digit.Select( d => d - '0' )
        .AtLeastOnce().Select( digs => new BatteryBank( [.. digs] ) );
}
