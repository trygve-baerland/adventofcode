
using System.Net.Sockets;
using AoC.Utils;
using CommandLine;
using Sprache;

namespace AoC.Y2025;

public sealed class Day2 : IPuzzle
{
    public IEnumerable<Interval<long>> TestData =
        Helpers.AllRanges.Parse( "2025/inputdata/day2_test.txt".GetText() ).ToList();

    public IEnumerable<Interval<long>> Data =
        Helpers.AllRanges.Parse( "2025/inputdata/day2.txt".GetText() ).ToList();

    public void Part1()
    {
        HashSet<long> invalids = new HashSet<long>();
        foreach ( var cand in Enumerable.Range( 1, 1000000 ) )
        {
            var tmp = Helpers.MakeInvalids( cand, 12, 2 ).ToList();
            foreach ( var c in tmp )
            {
                if ( Data.Any( r => r.Contains( c ) ) )
                {
                    invalids.Add( c );
                }
            }
        }
        Console.WriteLine( invalids.Sum() );
    }

    public void Part2()
    {

        HashSet<long> invalids = new HashSet<long>();
        Console.WriteLine( string.Join( ',', Helpers.MakeInvalids( 74, 8, 4 ) ) );
        foreach ( var cand in Enumerable.Range( 1, 1000000 ) )
        {
            var tmp = Helpers.MakeInvalids( cand, 12, 10 ).ToList();
            foreach ( var c in tmp )
            {
                if ( Data.Any( r => r.Contains( c ) ) )
                {
                    invalids.Add( c );
                }
            }
        }
        Console.WriteLine( invalids.Sum() );
    }
}

static partial class Helpers
{

    public static readonly Parser<Interval<long>> Range =
        Long.Then( num1 => Parse.Char( '-' ).Select( _ => num1 ) )
            .Then( num1 => Long.Select( num2 => new Interval<long>( num1, num2 ) ) );

    public static readonly Parser<IEnumerable<Interval<long>>> AllRanges =
        Range.DelimitedBy( Parse.Char( ',' ) );


    public static IEnumerable<long> MakeInvalids( long num, int maxDigits, int maxReps )
    {
        var ndigs = ( long ) System.Math.Log10( num ) + 1;
        var pow = ( long ) System.Math.Pow( 10, ndigs );
        var reps = 2;
        long tmp = num;
        while ( ndigs * reps <= maxDigits && reps <= maxReps )
        {
            tmp = pow * tmp + num;
            yield return tmp;
            reps += 1;
        }
    }
}
