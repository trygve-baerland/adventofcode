using System.Data;
using Sprache;
using Utils;
namespace AoC;

public sealed class Day12 : IPuzzle
{
    public List<SpringRecord> TestRecords { get; } = "inputdata/day12_test.txt".Stream().GetLines().Select( SpringRecord.FromString ).ToList();
    public List<SpringRecord> ActualRecords { get; } = "inputdata/day12.txt".Stream().GetLines().Select( SpringRecord.FromString ).ToList();

    public void Part1()
    {
        long result = ActualRecords.Sum( record => {
            return record.NumberOfValid();
        } );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var result = ActualRecords
        .Select( record => {
            var num = record.Repeat( 5 ).NumberOfValid();
            return num;
        } )
        .Sum();
        Console.WriteLine( result );
    }
}

public class LookupTable( char[] springConditions, int[] arrangement )
{
    private long?[][][] _table =
        Enumerable.Range( 0, springConditions.Length + 1 ).Select( _ =>
            Enumerable.Range( 0, 3 ).Select( _ =>
                Enumerable.Repeat( ( long? ) null, arrangement.Length + 1 ).ToArray()
            ).ToArray()
        ).ToArray();

    private int charToIndex( char c ) => c switch {
        '.' => 0,
        '#' => 1,
        '?' => 2,
        _ => throw new Exception( $"Invalid char '{c}'" )
    };

    public long? GetItem( RecordValidatationState state ) =>
        _table[state.Index][charToIndex( state.Current )][state.ConsumedBlocks];

    public void SetItem( RecordValidatationState state, long? value )
    {
        _table[state.Index][charToIndex( state.Current )][state.ConsumedBlocks] = value;
    }
}

public class SpringRecord( char[] springConditions, int[] arrangement )
{
    public char[] Conditions { get; } = springConditions;
    public int[] Arrangement { get; } = arrangement;

    public static SpringRecord FromString( string line ) =>
        Helpers.SpringRecordParser.Parse( line );

    public SpringRecord Repeat( int times )
    {
        var conditionString = string.Concat( Conditions );
        var newConditions = string.Join( '?', Enumerable.Repeat( conditionString, times ) ).ToCharArray();
        var newArrangement = Enumerable.Repeat( Arrangement, times ).SelectMany( x => x ).ToArray();
        return new SpringRecord( newConditions, newArrangement );
    }

    private LookupTable Cache2 { get; } = new LookupTable( springConditions, arrangement );

    public char AtIndex( int index )
    {
        if ( index >= Conditions.Length )
            return '.';
        return Conditions[index];
    }

    public override string ToString()
    {
        return $"{string.Concat( Conditions )} {string.Join( ',', Arrangement )}";
    }


    public bool CanHaveBlock( int index, int blockSize )
    {
        return Enumerable.Range( index, blockSize ).All( CanBeBlock ) && CanBeSpace( index + blockSize );
    }

    public bool CanBeBlock( int index )
    {
        var val = AtIndex( index );
        return val == '#' || val == '?';
    }

    public bool CanBeSpace( int index )
    {
        var val = AtIndex( index );
        return val == '.' || val == '?';
    }

    public int StrideToNextBlock( int index )
    {
        var counter = index + 1;
        while ( counter < Conditions.Length && !CanBeBlock( counter ) )
        {
            counter++;
        }
        return counter;
    }

    public long NumberOfValid()
    {
        return ValidateFromState(
            new RecordValidatationState( Conditions[0], 0, 0 )
        );
    }

    public long ValidateFromState( RecordValidatationState state )
    {
        if ( Cache2.GetItem( state ) is long value )
        {
            return value;
        }
        var result = _ValidateFromState( state );
        Cache2.SetItem( state, result );
        return result;
    }

    private bool MightBeValid( RecordValidatationState state )
    {
        // The number of remaing '#' or '?' we need are:
        var remaining = Arrangement[state.ConsumedBlocks..].Sum();
        var remainingSpaces = Arrangement.Length - state.ConsumedBlocks - 1;
        var available = Conditions[state.Index..].Count( c => c == '#' || c == '?' );
        return (remaining <= available) && (remainingSpaces + remaining <= Conditions.Length - state.Index);
    }
    private long _ValidateFromState( RecordValidatationState state )
    {
        // Various validations to stop recursion:
        // 1. When we're at the end:
        if ( state.Index >= Conditions.Length )
        {
            // Check that all blocks have been consumed
            if ( state.ConsumedBlocks == Arrangement.Length )
            {
                return 1;
            }
            return 0;
        }

        // 2. We have consumed more blocks than allowed:
        if ( state.ConsumedBlocks > Arrangement.Length )
            return 0;
        // 3. Do we have enough remaining space:
        if ( !MightBeValid( state ) )
            return 0;

        switch ( state.Current )
        {
            case '.':
                // Next index:
                var newIndex = StrideToNextBlock( state.Index );
                var newChar = AtIndex( newIndex );
                return ValidateFromState(
                    new RecordValidatationState( newChar, newIndex, state.ConsumedBlocks )
                );
            case '#':
                // Check if we can have a block here:
                if ( state.ConsumedBlocks < Arrangement.Length && CanHaveBlock( state.Index, Arrangement[state.ConsumedBlocks] ) )
                {
                    // Next index:
                    var nextIndex = state.Index + Arrangement[state.ConsumedBlocks];
                    return ValidateFromState(
                        new RecordValidatationState( '.', nextIndex, state.ConsumedBlocks + 1 )
                    );
                }
                else
                {
                    // We can't have a block here, so we can't consume it:
                    return 0;
                }
            case '?':
                // Can be either . or #
                return ValidateFromState(
                    new RecordValidatationState( '#', state.Index, state.ConsumedBlocks )
                ) + ValidateFromState(
                    new RecordValidatationState( '.', state.Index, state.ConsumedBlocks )
                );

            default:
                throw new Exception( $"Invalid token '{state.Current}'" );
        }
    }
}

public struct RecordValidatationState(
    char current,
    int index,
    int consumedBlocks
)
{
    // Current character
    public char Current { get; set; } = current;
    // Index of condition array we are at
    public int Index { get; set; } = index;
    // Number of blocks consumed
    public int ConsumedBlocks { get; set; } = consumedBlocks;

    public override string ToString()
    {
        return $"Current: {Current}, Index: {Index}, ConsumedBlocks: {ConsumedBlocks}";
    }
}

public static partial class Helpers
{
    #region parsers
    public static readonly Parser<char> SpringCondition =
        Parse.Chars( '.', '#', '?' );

    public static readonly Parser<char[]> SpringConditions =
        SpringCondition.AtLeastOnce().Select( chars => chars.ToArray() );

    public static readonly Parser<int> ArrangeMent =
        Parse.Number.Select( int.Parse );

    public static readonly Parser<int[]> Arrangements =
        ArrangeMent.DelimitedBy( Parse.Char( ',' ) ).Select( ints => ints.ToArray() );

    public static readonly Parser<SpringRecord> SpringRecordParser =
        SpringConditions
        .Then( conditions => Parse.Char( ' ' ).Select( _ => conditions ) )
        .Then( conditions => Arrangements.Select(
            arrangement => new SpringRecord( conditions, arrangement )
        ) );
    #endregion parsers
}
