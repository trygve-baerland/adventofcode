using AoC.Utils;

namespace AoC.Y2022;

public sealed class Day6 : IPuzzle
{
    private IEnumerable<char> TestInput() =>
        "2022/inpudata/day6_test.txt".Stream().GetChars();

    private IEnumerable<char> ActualInput() =>
        "2022/inputdata/day6.txt".Stream().GetChars();
    public void Part1()
    {
        var enumerator = ActualInput().GetEnumerator();
        enumerator.MoveNext();
        var buffer = enumerator.Take( 4 ).ToArray();
        int counter = 3;
        while ( enumerator.MoveNext() )
        {
            if ( !Day6Helpers.Distinct( buffer ) )
            {
                counter++;
            }
            else
            {
                break;
            }
            buffer[counter % 4] = enumerator.Current;

        }
        Console.WriteLine( $"Result: {counter + 1}" );
    }

    public void Part2()
    {
        var enumerator = ActualInput().GetEnumerator();
        enumerator.MoveNext();
        var buffer = enumerator.Take( 14 ).ToArray();
        int counter = 13;
        while ( enumerator.MoveNext() )
        {
            if ( !Day6Helpers.Distinct( buffer ) )
            {
                counter++;
            }
            else
            {
                break;
            }
            buffer[counter % 14] = enumerator.Current;

        }
        Console.WriteLine( $"Result: {counter + 1}" );
    }
}

internal static class Day6Helpers
{
    public static bool Distinct( char[] arr )
    {
        return arr.Distinct().Count() == arr.Count();
    }
}

