using AoC.Utils;
using Sprache;
namespace AoC.Y2023;

public sealed class Day5 : IPuzzle
{
    public string TestData = string.Concat( "2023/inputdata/day5_test.txt".Stream().GetChars() );
    public string ActualData = string.Concat( "2023/inputdata/day5.txt".Stream().GetChars() );
    public void Part1()
    {
        var (seeds, mappings) = Helpers.InputFile.Parse( ActualData );

        var result = seeds
            .Select( seed => mappings.Aggregate( seed, ( x, mapping ) => mapping.Map( x ) ) )
            .Min();
        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var (seeds, mappings) = Helpers.InputFile.Parse( ActualData );

        var result = seeds.Clump( 2 )
            .Select( pair => new Interval<long>( pair.First(), pair.First() + pair.Skip( 1 ).First() - 1 ) ) // Create intervals
            .Select( interval =>
                mappings.Aggregate(
                    seed: new[] { interval } as IEnumerable<Interval<long>>,
                    func: ( i, m ) => m.Map( i ) )
            )
            .Flatten()
            .Select( i => i.A )
            .Min();

        Console.WriteLine( $"Result: {result}" );

    }
}

public class MappingItem( long destinationRangeStart, long sourceRangeStart, long rangeLength )
{
    public long DestinationRangeStart { get; } = destinationRangeStart;
    public long SourceRangeStart { get; } = sourceRangeStart;
    public long RangeLength { get; } = rangeLength;

    public override string ToString()
    {
        return $"{DestinationRangeStart} {SourceRangeStart} {RangeLength}";
    }

    public bool ContainsSource( long source )
    {
        return source >= SourceRangeStart && source < SourceRangeStart + RangeLength;
    }

    public long Map( long source ) => DestinationRangeStart + (source - SourceRangeStart);
}

public class Mapping( string destName, string sourceName, IEnumerable<MappingItem> items )
{
    public string SourceName { get; } = sourceName;
    public string DestName { get; } = destName;
    public List<MappingItem> Items { get; } = FixUpItems( items );

    public static List<MappingItem> FixUpItems( IEnumerable<MappingItem> items )
    {
        var origItems = items.OrderBy( item => item.SourceRangeStart ).ToList();
        List<MappingItem> newItems = [];

        var enumerator = origItems.GetEnumerator();
        enumerator.MoveNext();

        var prev = enumerator.Current;
        // Add a beginning interval:
        if ( prev.SourceRangeStart > 0 )
        {
            newItems.Add(
                new MappingItem(
                    destinationRangeStart: 0,
                    sourceRangeStart: 0,
                    rangeLength: prev.SourceRangeStart
                )
            );
        }
        newItems.Add( prev );
        while ( enumerator.MoveNext() )
        {
            var item = enumerator.Current;
            if ( item.SourceRangeStart > prev.SourceRangeStart + prev.RangeLength )
            {
                newItems.Add(
                    new MappingItem(
                        destinationRangeStart: prev.SourceRangeStart + prev.RangeLength,
                        sourceRangeStart: prev.SourceRangeStart + prev.RangeLength,
                        rangeLength: item.SourceRangeStart - prev.SourceRangeStart
                    )
                );
            }
            newItems.Add( item );
            prev = item;
        }
        // Finally, add an end element:
        var endPoint = prev.SourceRangeStart + prev.RangeLength;
        newItems.Add(
            new MappingItem(
                destinationRangeStart: endPoint,
                sourceRangeStart: endPoint,
                rangeLength: long.MaxValue - endPoint
            )
        );
        return [.. newItems.OrderBy( item => item.SourceRangeStart )];
    }

    public override string ToString()
    {
        var result = $"{SourceName}-to-{DestName}:\n";
        result += string.Join( '\n', Items );
        return result;
    }

    public long Map( long source )
    {
        var mapping = Items.FirstOrDefault( item => item!.ContainsSource( source ), null );
        if ( mapping is not null )
        {
            return mapping.Map( source );
        }
        return source;
    }

    public IEnumerable<Interval<long>> Map( Interval<long> interval )
    {
        var start = interval.A;
        var end = interval.B;

        var enumerator = Items.GetEnumerator();
        while ( enumerator.MoveNext() && start < end )
        {
            var item = enumerator.Current;
            if ( item.ContainsSource( start ) )
            {
                // "end" might be contained in the current interval:
                var xEnd = System.Math.Min( end, item.SourceRangeStart + item.RangeLength - 1 );
                // If xEnd <= end we are done with this interval
                var newInterval = new Interval<long>(
                    item.Map( start ),
                    item.Map( xEnd )
                );
                yield return newInterval;
                start = xEnd + 1;
            }
        }
    }

    public IEnumerable<Interval<long>> Map( IEnumerable<Interval<long>> intervals ) =>
        intervals.Select( Map ).Flatten();
}

public static partial class Helpers
{
    #region parser stuff
    public static readonly Parser<long> Long =
    Parse.Optional( Parse.Char( '-' ) ).Select( sign => sign.IsDefined ? -1 : 1 )
        .Then( sign => Parse.Number.Select( num => sign * long.Parse( num ) ) );
    public static readonly Parser<IEnumerable<long>> SeedNumbers =
        Parse.String( "seeds: " )
        .Then( _ => Long.DelimitedBy( Parse.Char( ' ' ).AtLeastOnce() ) )
        .Then( seeds => Parse.LineEnd.Select( _ => seeds ) );

    public static readonly Parser<MappingItem> MappingItem =
        Long
        .Then( dest => Parse.Char( ' ' ).AtLeastOnce().Select( _ => dest ) )
        .Then( dest => Long.Select( source => (dest, source) ) )
        .Then( item => Parse.Char( ' ' ).AtLeastOnce().Select( _ => item ) )
        .Then( item => Long.Select( range => (item.dest, item.source, range) ) )
        .Select( item => new MappingItem( item.dest, item.source, item.range ) );

    public static readonly Parser<Mapping> Mapping =
        Parse.Char( char.IsLetter, "source name" ).Many().Select( chars => string.Concat( chars ) )
        .Then( sourceName => Parse.String( "-to-" ).Select( _ => sourceName ) )
        .Then( sourceName => Parse.Char( char.IsLetter, "dest name" ).Many()
            .Select( chars => string.Concat( chars ) )
            .Select( destName => (destName, sourceName) )
        )
        .Then( item => Parse.String( " map:" ).Then( _ => Parse.LineEnd ).Select( _ => item ) )
        .Then( item => MappingItem.DelimitedBy( Parse.LineEnd ).Select( items =>
            new Mapping( item.destName, item.sourceName, items )
        ) );

    public static readonly Parser<(List<long> seeds, List<Mapping> mappings)> InputFile =
        SeedNumbers.Select( seeds => seeds.ToList() )
        .Then( seeds => Parse.LineEnd.AtLeastOnce().Select( _ => seeds ) )
        .Then( seeds =>
            Mapping.DelimitedBy( Parse.LineEnd.AtLeastOnce() )
            .Select( mappings => (seeds, mappings: mappings.ToList()) )
        );

    #endregion parser stuff
}
