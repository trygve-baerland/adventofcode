using Sprache;
using Utils;
namespace AoC;

public sealed class Day12 : IPuzzle
{
    public List<SpringRecord> TestRecords { get; } = "inputdata/day12_test.txt".Stream().GetLines().Select( SpringRecord.FromString ).ToList();
    public List<SpringRecord> ActualRecords { get; } = "inputdata/day12.txt".Stream().GetLines().Select( SpringRecord.FromString ).ToList();

    public void Part1()
    {
        long result = ActualRecords.Sum( record => record.NumberOfValid() );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var result = ActualRecords.AsParallel()
        .Select( record => {
            var num = record.Repeat( 5 ).NumberOfValid();
            Console.WriteLine( $"{record} => {num}" );
            return num;
        } )
        .Sum();
        Console.WriteLine( result );
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

    public long NumberOfValid()
    {
        return ValidateFromState(
            new RecordValidatationState( Conditions[0], 0, false, 0, 0 )
        );
    }

    public long ValidateFromState( RecordValidatationState state )
    {
        // Various validations to stop recursion:

        // 1. When we're at the end:
        if ( state.Index == Conditions.Length )
        {
            if ( state.InBlock )
            {
                // We are done consuming a block.
                // We need to verify that it had the proper length:
                if ( state.ConsumedBlocks >= Arrangement.Length )
                {
                    return 0;
                }
                if ( state.Current == '#' ) state.CurrentlyConsumed++;
                if ( state.CurrentlyConsumed != Arrangement[state.ConsumedBlocks] )
                {
                    return 0;
                }
                state.ConsumedBlocks++;
            }
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

        // Get next item:
        char next = AtIndex( state.Index + 1 );
        switch ( state.Current )
        {
            case '.':
                if ( state.InBlock )
                {
                    // We are done consuming a block.
                    // We need to verify that it had the proper length:
                    if ( state.CurrentlyConsumed != Arrangement[state.ConsumedBlocks] )
                    {
                        return 0;
                    }
                    return ValidateFromState(
                        new RecordValidatationState( next, state.Index + 1, false, 0, state.ConsumedBlocks + 1 )
                    );
                }
                return ValidateFromState(
                    new RecordValidatationState( next, state.Index + 1, false, 0, state.ConsumedBlocks )
                );
            case '#':
                // Add to consumed
                if ( state.InBlock )
                {
                    // If we haven't overconsumed, we can continue:
                    if ( state.CurrentlyConsumed < Arrangement[state.ConsumedBlocks] )
                    {
                        return ValidateFromState(
                            new RecordValidatationState( next, state.Index + 1, true, state.CurrentlyConsumed + 1, state.ConsumedBlocks )
                        );
                    }
                    return 0;
                }
                else
                {
                    // Starting a new block:
                    if ( state.ConsumedBlocks >= Arrangement.Length )
                        return 0;
                    return ValidateFromState(
                        new RecordValidatationState( next, state.Index + 1, true, 1, state.ConsumedBlocks )
                    );
                }
            case '?':
                // Can be either . or #
                return ValidateFromState(
                    new RecordValidatationState( '.', state.Index, state.InBlock, state.CurrentlyConsumed, state.ConsumedBlocks )
                ) + ValidateFromState(
                    new RecordValidatationState( '#', state.Index, state.InBlock, state.CurrentlyConsumed, state.ConsumedBlocks )
                );

            default:
                throw new Exception( $"Invalid token '{state.Current}'" );
        }
    }
}

public struct RecordValidatationState(
    char current,
    int index,
    bool inBlock,
    int CurrentlyConsumed,
    int consumedBlocks
)
{
    // Current character
    public char Current { get; set; } = current;
    // Index of condition array we are at
    public int Index { get; set; } = index;
    public bool InBlock { get; set; } = inBlock;
    // Consumed in current block
    public int CurrentlyConsumed { get; set; } = CurrentlyConsumed;
    // Number of blocks consumed
    public int ConsumedBlocks { get; set; } = consumedBlocks;
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
