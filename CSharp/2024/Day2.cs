using Sprache;
using AoC.Utils;
namespace AoC.Y2024;

public sealed class Day2 : IPuzzle
{
    private IEnumerable<Report> TestReports = "2024/inputdata/day2_test.txt"
        .GetLines()
        .Select( Report.FromString ).ToList();

    private IEnumerable<Report> Reports = "2024/inputdata/day2.txt"
        .GetLines()
        .Select( Report.FromString ).ToList();
    public void Part1()
    {
        var result = Reports.Where( rep => rep.IsSafe() ).Count();
        Console.WriteLine( $"{result}" );
    }

    public void Part2()
    {
        var result = Reports.Where( rep => rep.IsSafeWithDampener() ).Count();
        Console.WriteLine( $"{result}" );
    }
}

struct Report
{
    public List<long> Levels { get; init; }

    public static Report FromString( string input )
    {
        return new Report {
            Levels = Longs.Parse( input )
        };
    }

    public override string ToString()
    {
        return string.Join( ',', Levels );
    }

    public bool IsSafe()
    {
        return IsSafe( Levels );
    }

    private static bool IsSafe( IEnumerable<long> levels )
    {
        var diffs = levels.Zip( levels.Skip( 1 ) )
            .Select( tup => tup.Second - tup.First )
            .ToList();
        // Check that all are less than 3, and of the same sign
        return diffs.All( item => long.Abs( item ) <= 3 && long.Abs( item ) > 0 ) &&
            (diffs.All( item => item <= 0 ) || diffs.All( item => item >= 0 ));
    }

    public bool IsSafeWithDampener()
    {
        foreach ( var k in Enumerable.Range( 0, Levels.Count() ) )
        {
            if ( IsSafe( Levels.IgnoreAt( k ) ) )
            {
                return true;
            }
        }
        return false;
    }


    private static Parser<List<long>> Longs = Parse.Number
        .Select( s => long.Parse( s ) ).DelimitedBy( Parse.WhiteSpace.AtLeastOnce() )
        .Select( longs => longs.ToList() );
}
