using System.Text;
using Sprache;
using AoC.Utils;
namespace AoC.Y2023;

public sealed class Day13 : IPuzzle
{
    public List<MirrorPattern> TestPatterns { get; } = Helpers.MirrorPatterns.Parse( string.Concat( "2023/inputdata/day13_test.txt".Stream().GetChars() ) ).ToList();
    public List<MirrorPattern> ActualPatterns { get; } = Helpers.MirrorPatterns.Parse( string.Concat( "2023/inputdata/day13.txt".Stream().GetChars() ) ).ToList();
    public void Part1()
    {
        long result = ActualPatterns
            .Select( pattern => pattern.ReflectionNumber( 0 ) )
            .Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        long result = ActualPatterns
            .Select( pattern => pattern.ReflectionNumber( 1 ) )
            .Sum();
        Console.WriteLine( result );
    }
}

public class MirrorPattern( char[][] pattern )
{
    public char[][] Pattern { get; } = pattern;

    public char this[Point p] => Pattern[p.Y][p.X];

    public int Width => Pattern[0].Length;
    public int Height => Pattern.Length;

    public override string ToString()
    {
        var sb = new StringBuilder();
        Pattern.Select( ( e, i ) => (e, i) ).ForEach( item => sb.AppendLine( $"{item.i}: {new string( item.e )}" ) );
        return sb.ToString();
    }

    public List<char> Row( int index ) => Pattern[index].ToList();
    public IEnumerable<List<char>> Rows() => Pattern.Select( row => row.ToList() );
    public List<char> Column( int index ) => Pattern.Select( row => row[index] ).ToList();
    public IEnumerable<List<char>> Columns() => Enumerable.Range( 0, Width ).Select( Column );

    public static int IsReflected( List<char> symbols, int index )
    {
        return symbols[0..index].OtherWay().Zip( symbols[index..^0] )
        .Where( item => item.First != item.Second )
        .Count();
    }

    public long ReflectionNumber( int target = 0 )
    {
        // Go through each column:
        var result = Enumerable.Range( 1, Width - 1 )
            .Where( index => Rows()
                .Select( row => IsReflected( row, index ) )
                .Sum() == target
            )
            .FirstOrDefault( defaultValue: 0 );
        if ( result > 0 )
        {
            return result;
        }
        // Go through each row:
        return 100 * Enumerable.Range( 1, Height - 1 )
            .Where( index => Columns()
                .Select( column => IsReflected( column, index ) )
                .Sum() == target
            )
            .FirstOrDefault( defaultValue: 0 );

    }
}
public static partial class Helpers
{
    #region parsers
    public static readonly Parser<char> MirrorSymbol = Parse.Chars( '.', '#' );

    public static readonly Parser<char[]> MirrorRow =
        MirrorSymbol.AtLeastOnce().Select( x => x.ToArray() );

    public static readonly Parser<MirrorPattern> MirrorPattern =
        MirrorRow.DelimitedBy( Parse.Char( '\n' ) ).Select( rows => new MirrorPattern( rows.ToArray() ) );

    public static readonly Parser<IEnumerable<MirrorPattern>> MirrorPatterns =
        MirrorPattern.DelimitedBy( Parse.LineEnd.AtLeastOnce() );
    #endregion parsers
}
