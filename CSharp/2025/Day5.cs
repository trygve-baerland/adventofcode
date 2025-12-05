
using System.Text.Unicode;
using AoC.Utils;
using Sprache;

namespace AoC.Y2025;

public sealed class Day5 : IPuzzle
{
    private (List<Interval<long>>, List<long>) TestData =
        Helpers.RangesAndIdsParser.Parse( "2025/inputdata/day5_test.txt".GetText() );
    private (List<Interval<long>>, List<long>) Data =
        Helpers.RangesAndIdsParser.Parse( "2025/inputdata/day5.txt".GetText() );

    public void Part1()
    {
        var data = Data;
        var result = data.Item2.Where( id => data.Item1.Any( r => r.Contains( id ) ) ).Count();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var result = Data.Item1.ToHashSet()
            .FixPoint( Helpers.SimplifyRangesSet )
            .TakeTwo()
            .TakeWhile( pair => pair.Item1.Count() != pair.Item2.Count() )
            .Last()
            .Item2
            .Select( r => r.Count() ).Sum();

        Console.WriteLine( result );

    }
}

static partial class Helpers
{
    public static readonly Parser<(List<Interval<long>>, List<long>)> RangesAndIdsParser =
        Range.DelimitedBy( Parse.LineEnd ).Select( ranges => ranges.ToList() )
        .Then( ranges => Parse.LineEnd.AtLeastOnce().Select( _ => ranges ) )
        .Then( ranges => Long.DelimitedBy( Parse.LineEnd ).Select( ids => (ranges, ids.ToList()) ) );

    public static HashSet<Interval<long>> SimplifyRangesSet( HashSet<Interval<long>> ranges )
    {
        var intersections = ranges.Pairs().Where( pair => pair.Item1.Intersects( pair.Item2 ) )
            .Select( pair => pair.Item1.Union( pair.Item2 ) )
            .ToHashSet();

        var isolated = ranges.Where( r => !intersections.Any( r2 => r2.Intersects( r ) ) );
        return isolated.Union( intersections ).ToHashSet();
    }

}
