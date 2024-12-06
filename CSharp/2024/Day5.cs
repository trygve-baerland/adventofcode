using AoC.Utils;
using Sprache;

namespace AoC.Y2024;

public sealed class Day5 : IPuzzle
{
    private readonly UpdateLog TestLog = UpdateLog.FromFile( "2024/inputdata/day5_test.txt" );
    private readonly UpdateLog Log = UpdateLog.FromFile( "2024/inputdata/day5.txt" );
    public void Part1()
    {
        var log = Log;
        Console.WriteLine( $"Number of rules: {log.Rules.Count}" );
        Console.WriteLine( $"Number of entries: {log.Entries.Count}" );
        var result = log.Entries.Where( entry => log.Rules.All( entry.CheckRule ) )
            .Select( entry => ( long ) entry.Middle() )
            .Sum();

        Console.WriteLine( result );
    }

    public void Part2()
    {
        var log = Log;
        var result = log.Entries.Where( entry => log.Rules.Any( rule => !entry.CheckRule( rule ) ) )
            .Select( entry => {
                entry.Pages.Sort( ( p1, p2 ) => log.CompareByRules( p1, p2 ) );
                return entry.Middle();
            } )
            .Sum();
        Console.WriteLine( result );
    }
}

internal record struct OrderingRule
{
    public int Before { get; init; }
    public int After { get; init; }

    public int CompareByRule( int page1, int page2 )
    {
        if ( page1 == Before && page2 == After )
        {
            return 1;
        }
        if ( page2 == Before && page1 == After )
        {
            return -1;
        }
        return 0;
    }
}

internal record class UpdateEntry
{
    public List<int> Pages { get; init; } = new();

    public bool CheckRule( OrderingRule rule )
    {
        var ib = Pages.FindIndex( p => p == rule.Before );
        var ia = Pages.FindIndex( p => p == rule.After );
        ia = ia < 0 ? Pages.Count : ia;
        return ib < ia;
    }

    public int Middle() => Pages[Pages.Count / 2];
}

internal record class UpdateLog
{
    public List<OrderingRule> Rules { get; init; } = new();
    public List<UpdateEntry> Entries { get; init; } = new();

    public static UpdateLog FromFile( string fileName )
    {
        return Helpers.ALog.Parse( fileName.GetText() );
    }

    public int CompareByRules( int page1, int page2 )
    {
        return Rules.Select( rule => rule.CompareByRule( page1, page2 ) )
            .FirstOrDefault( val => val != 0 );
    }
}
internal static partial class Helpers
{
    public static readonly Parser<int> Int = Parse.Number.Select( int.Parse );
    public static readonly Parser<OrderingRule> ARule =
        Int.Then( b => Parse.Char( '|' ).Select( _ => b ) )
            .Then( b => Int.Select( a => new OrderingRule { Before = b, After = a } ) );

    public static readonly Parser<UpdateEntry> AnEntry =
        Int.DelimitedBy( Parse.Char( ',' ) )
            .Select( items => new UpdateEntry { Pages = items.ToList() } );

    public static readonly Parser<UpdateLog> ALog =
        ARule.DelimitedBy( Parse.LineEnd )
        .Then( rules => Parse.LineEnd.Many().Select( _ => rules ) )
        .Then( rules => AnEntry.DelimitedBy( Parse.LineEnd )
            .Select( entries => new UpdateLog {
                Rules = rules.ToList(),
                Entries = entries.ToList()
            } )
        );
}
