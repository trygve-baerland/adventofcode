using AoC.Utils;
namespace AoC.Y2022;


public sealed class Day3 : IPuzzle
{
    public IEnumerable<string> TestLines() => "2022/inputdata/day3_test.txt".GetLines();
    public IEnumerable<string> ActualLines() => "2022/inputdata/day3.txt".GetLines();

    public void Part1()
    {
        var result = ActualLines()
            .Select( Day3Helpers.SplitLine )
            .Select( tup => tup.lhs.Intersect( tup.rhs ).First() )
            .Select( Day3Helpers.Score )
            .Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var result = ActualLines()
            .Clump( 3 )
            .Select( Day3Helpers.GetCommonLetter )
            .Select( Day3Helpers.Score )
            .Sum();
        Console.WriteLine( result );
    }
}

internal static class Day3Helpers
{
    public static (char[] lhs, char[] rhs) SplitLine( string line )
    {
        var len = line.Length;
        return (
            line.ToCharArray( 0, len / 2 ),
            line.ToCharArray( len / 2, len / 2 )
        );
    }
    public static int Score( char letter )
    {
        if ( char.IsUpper( letter ) )
        {
            return letter - 'A' + 27;
        }
        else
        {
            return letter - 'a' + 1;
        }
    }

    public static char GetCommonLetter( IEnumerable<string> lines )
    {
        return lines.Select( line => line.ToCharArray() )
            .Aggregate( ( acc, item ) => acc.Intersect( item ).ToArray() )
            .First();
    }
}
