using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Utils;

namespace AoC;

public sealed class Day3 : IPuzzle
{
    public char[][] TestData = "inputdata/day3_test.txt".GetLines()
        .Select( line => line.ToCharArray() ).ToArray();
    public char[][] ActualData = "inputdata/day3.txt".GetLines()
        .Select( line => line.ToCharArray() ).ToArray();

    public void Part1()
    {
        var result = MachineNumbers( ActualData ).Sum();
        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var result = GearRatios( ActualData ).Sum();
        Console.WriteLine( $"Result: {result}" );
    }

    #region methods
    private static IEnumerable<(char symbol, int x, int y)> SymbolCoords( char[][] schematic ) =>
        schematic.Select( ( row, i ) => row.Select( ( col, j ) => (col, i, j) ) )
        .SelectMany( x => x )
        .Where( item => (!Char.IsDigit( item.col )) && (item.col != '.') );

    private static IEnumerable<int> AdjacentNumbers( char[][] schematic, int i, int j )
    {
        var NR = schematic.Length;
        var NC = schematic[0].Length;

        // Above symbol
        if ( i - 1 >= 0 )
        {
            if ( j - 1 >= 0 && char.IsAsciiDigit( schematic[i - 1][j - 1] ) )
            {
                yield return GetNumberFrom( schematic, i - 1, j - 1 );
            }
            else if ( char.IsDigit( schematic[i - 1][j] ) )
            {
                yield return GetNumberFrom( schematic, i - 1, j );
            }
            if ( j + 1 < NC &&
                char.IsDigit( schematic[i - 1][j + 1] ) &&
                !char.IsDigit( schematic[i - 1][j] ) )
            {
                yield return GetNumberFrom( schematic, i - 1, j + 1 );
            }
        }
        // Below symbol
        if ( i + 1 < NR )
        {
            if ( j - 1 >= 0 && Char.IsAsciiDigit( schematic[i + 1][j - 1] ) )
            {
                yield return GetNumberFrom( schematic, i + 1, j - 1 );
            }
            else if ( Char.IsDigit( schematic[i + 1][j] ) )
            {
                yield return GetNumberFrom( schematic, i + 1, j );
            }
            if ( j + 1 < NC &&
                char.IsDigit( schematic[i + 1][j + 1] ) &&
                !char.IsDigit( schematic[i + 1][j] )
                )
            {
                yield return GetNumberFrom( schematic, i + 1, j + 1 );
            }
        }

        // Left of symbol
        if ( j - 1 >= 0 && Char.IsDigit( schematic[i][j - 1] ) )
        {
            yield return GetNumberFrom( schematic, i, j - 1 );
        }
        // Right of symbol
        if ( j + 1 < NC && Char.IsDigit( schematic[i][j + 1] ) )
        {
            yield return GetNumberFrom( schematic, i, j + 1 );
        }
    }

    private static int GetNumberFrom( char[][] schematic, int i, int j )
    {
        // There's a digit at (i,j). We need to find the start of it:
        var current = j;
        while ( char.IsDigit( schematic[i][current] ) )
        {
            current -= 1;
            if ( current < 0 )
            {
                break;
            }
        }
        var colIndex = current + 1;

        var number = 0;
        var NC = schematic[i].Length;
        while ( char.IsDigit( schematic[i][colIndex] ) )
        {
            number = 10 * number + (schematic[i][colIndex] - '0');
            colIndex += 1;
            if ( colIndex == NC )
            {
                break;
            }
        }
        return number;
    }

    private static IEnumerable<int> MachineNumbers( char[][] schematic ) =>
        SymbolCoords( schematic ).Select( item => AdjacentNumbers( schematic, item.x, item.y ) )
            .SelectMany( x => x );

    private static IEnumerable<int> GearRatios( char[][] schematic ) =>
        SymbolCoords( schematic )
        .Where( item => item.symbol == '*' )
        .Select( item => AdjacentNumbers( schematic, item.x, item.y ).ToList() )
        .Where( numbers => numbers.Count == 2 )
        .Select( numbers => numbers[0] * numbers[1] );

    #endregion methods
}
