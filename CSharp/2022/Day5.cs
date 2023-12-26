using AoC.Utils;
using System.Text.RegularExpressions;

namespace AoC.Y2022;


public sealed class Day5 : IPuzzle
{
    private (Dictionary<int, Stack<char>>, IEnumerable<MoveSpec>) TestStacks() =>
        "2022/inputdata/day5_test.txt".GetLines().ParseStacks();
    private (Dictionary<int, Stack<char>>, IEnumerable<MoveSpec>) ActualStacks() =>
        "2022/inputdata/day5.txt".GetLines().ParseStacks();

    public void Part1()
    {
        var (stacks, moves) = ActualStacks();
        foreach ( var move in moves )
        {
            for ( int i = 0; i < move.Num; i++ )
            {
                stacks[move.To].Push( stacks[move.From].Pop() );
            }
        }

        foreach ( var (_, stack) in stacks )
        {
            Console.Write( stack.Peek() );
        }
        Console.WriteLine();
    }
    public void Part2()
    {
        var (stacks, moves) = ActualStacks();
        foreach ( var move in moves )
        {
            var tempStack = new Stack<char>();
            for ( int i = 0; i < move.Num; i++ )
            {
                tempStack.Push( stacks[move.From].Pop() );
            }
            while ( tempStack.Count > 0 )
            {
                stacks[move.To].Push( tempStack.Pop() );
            }
        }

        foreach ( var (_, stack) in stacks )
        {
            Console.Write( stack.Peek() );
        }
        Console.WriteLine();
    }
}

public record struct MoveSpec( int Num, int From, int To )
{
    public static readonly Regex Pattern = new( @"move (\d+) from (\d+) to (\d+)", RegexOptions.None );
    public static MoveSpec Parse( string line )
    {
        var match = Pattern.Match( line );
        int num = int.Parse( match.Groups[1].Value );
        int from = int.Parse( match.Groups[2].Value );
        int to = int.Parse( match.Groups[3].Value );
        return new MoveSpec( num, from, to );
    }
}
public static class Day5Helpers
{
    public static void PrintStack( Stack<char> stack )
    {
        while ( stack.Count > 0 )
        {
            Console.WriteLine( $"{stack.Pop()}, " );
        }
    }

    public static (Dictionary<int, Stack<char>>, IEnumerable<MoveSpec>) ParseStacks( this IEnumerable<string> lines )
    {
        // Parse initial stack:
        string line = "Initial";
        Stack<string> initStack = new();
        var enumerator = lines.GetEnumerator();
        while ( line != "" && enumerator.MoveNext() )
        {
            line = enumerator.Current;
            initStack.Push( line );
        }
        // Pop last, empty line:
        initStack.Pop();
        var binsLine = initStack.Pop();
        var binIndices = binsLine.ToCharArray()
            .Select( ( c, i ) => new { C = c, Index = i } )
            .Where( item => char.IsDigit( item.C ) )
            .Select( item => new { N = int.Parse( item.C.ToString() ), item.Index } )
            .ToDictionary(
                item => item.N,
                item => item.Index
            );

        // Initialize stacks
        var stacks = binIndices.ToDictionary(
            item => item.Key,
            item => new Stack<char>()
        );

        // Make initial stacks:
        while ( initStack.Count > 0 )
        {
            var toParse = initStack.Pop();
            // Go through each bin:
            foreach ( var item in binIndices )
            {
                // Get character at current line and index:
                char c = toParse[item.Value];
                // If it's a letter, we put it on the corresponding stack
                if ( char.IsAsciiLetter( c ) )
                {
                    stacks[item.Key].Push( c );
                }
            }
        }

        // Parse moves:
        var moves = lines.Select( MoveSpec.Parse );
        return (stacks, moves);
    }
}
