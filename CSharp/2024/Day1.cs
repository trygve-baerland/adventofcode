using AoC.Utils;
using Sprache;

namespace AoC.Y2024;

public sealed class Day1 : IPuzzle
{

    public IEnumerable<(long, long)> TestLines =
        "2024/inputdata/day1_test.txt".GetLines().Select( line => Helpers.Line.Parse( line ) ).ToArray();
    public IEnumerable<(long, long)> Lines =
        "2024/inputdata/day1.txt".GetLines().Select( line => Helpers.Line.Parse( line ) ).ToArray();
    public void Part1()
    {
        var col1 = Lines.Select( l => l.Item1 ).ToArray();
        Array.Sort( col1 );
        var col2 = Lines.Select( l => l.Item2 ).ToArray();
        Array.Sort( col2 );

        var result = col1.Zip( col2 ).Select( tup => long.Abs( tup.First - tup.Second ) ).Sum();
        Console.WriteLine( $"{result}" );
    }

    public void Part2()
    {
        var col1 = Lines.Select( l => l.Item1 ).ToArray();
        var col2 = Lines.Select( l => l.Item2 ).ToArray();
        Array.Sort( col2 );
        var mapping = col2.GroupBy( l => l ).Select( g => (g.Key, g.Count()) ).ToDictionary();

        var result = col1.Select( l => l * mapping.GetValueOrDefault( l, 0 ) ).Sum();

        Console.WriteLine( $"{result}" );

    }
}

static partial class Helpers
{
    private static readonly Parser<long> Long = Parse.Number.Select( long.Parse );
    public static readonly Parser<(long, long)> Line =
        Long.Then( l1 => Parse.Char( ' ' ).Many().Select( _ => l1 ) )
        .Then( l1 => Long.Select( l2 => (l1, l2) ) );
}
