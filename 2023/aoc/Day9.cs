using Sprache;
using Utils;

namespace AoC;

public sealed class Day9 : IPuzzle
{
    public IEnumerable<History> TestHistories { get; } = Helpers.AllHistories.Parse( string.Concat( "inputdata/day9_test.txt".Stream().GetChars() ) );
    public IEnumerable<History> ActualHistories { get; } = Helpers.AllHistories.Parse( string.Concat( "inputdata/day9.txt".Stream().GetChars() ) );
    public void Part1()
    {
        var result = ActualHistories.Select( history => history.Extrapolate().after ).Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var result = ActualHistories.Select( history => history.Extrapolate().before ).Sum();
        Console.WriteLine( result );
    }
}

public class History( IEnumerable<long> values )
{
    public long[] Values { get; } = values.ToArray();
    public override string ToString()
    {
        return string.Join( ", ", Values );
    }

    public (long before, long after) Extrapolate()
    {
        // Get differences
        var stack = new Stack<long[]>();
        stack.Push( Values );
        var current = stack.Peek();
        while ( current.Any( v => v != 0 ) )
        {
            stack.Push( current.Zip( current.Skip( 1 ), ( a, b ) => b - a ).ToArray() );
            current = stack.Peek();
        }
        long after = 0;
        long before = 0;
        while ( stack.Count > 0 )
        {
            var diffs = stack.Pop();
            after += diffs.Last();
            before = diffs.First() - before;
        }
        return (before, after);
    }
}

public partial class Helpers
{
    #region parser stuff
    public static readonly Parser<History> HistoryParser =
        Long.DelimitedBy( Parse.Char( ' ' ).AtLeastOnce() ).Select( values => new History( values ) );

    public static readonly Parser<IEnumerable<History>> AllHistories =
        HistoryParser.DelimitedBy( Parse.LineEnd );
    #endregion parser stuff
}
