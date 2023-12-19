using Utils;
using Sprache;
using System.Reflection.PortableExecutable;

namespace AoC;

public sealed class Day19 : IPuzzle
{
    public (AllPipelines pipelines, IEnumerable<MachinePart> parts) TestInput() => Helpers.ParseInput( "inputdata/day19_test.txt" );
    public (AllPipelines pipelines, IEnumerable<MachinePart> parts) ActualInput() => Helpers.ParseInput( "inputdata/day19.txt" );

    public void Part1()
    {
        var (pipelines, parts) = ActualInput();
        var result = parts.Where( pipelines.Accept ).Sum( part => part.Score() );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var (pipelines, parts) = ActualInput();
        var result = pipelines["in"].ValidRange( pipelines ).Select( range => range.Score() ).Sum();
        Console.WriteLine( result );
    }
}

public record struct MachinePart( int x, int m, int a, int s )
{
    public override string ToString()
    {
        return $"{{x={x},m={m},a={a},s={s}}}";
    }

    public long Score() => x + m + a + s;

    public int Get( char attr ) => attr switch {
        'x' => x,
        'm' => m,
        'a' => a,
        's' => s,
        _ => throw new ArgumentException( $"Unknown attribute {attr}" )
    };
}

public record struct Pipeline( string Name, List<IRule> rules )
{
    public override string ToString()
    {
        return $"{Name}: {string.Join( ", ", rules )}";
    }

    public IEnumerable<MachinePartRange> ValidRange( AllPipelines context )
        => rules[0].ValidRange( this, context, 0 );
}
public record struct AllPipelines( Dictionary<string, Pipeline> pipelines )
{
    public Pipeline this[string name] => pipelines[name];

    public bool Accept( MachinePart part ) => pipelines["in"].rules[0].ValidateNext( part, pipelines["in"], this, 0 ) is AcceptRule;
}

public interface IRule
{
    public IRule ValidateNext( MachinePart part, Pipeline current, AllPipelines context, int index );
    public IEnumerable<MachinePartRange> ValidRange( Pipeline current, AllPipelines context, int index );

}

public struct AcceptRule : IRule
{
    public IRule ValidateNext( MachinePart part, Pipeline current, AllPipelines context, int index ) => this;

    public IEnumerable<MachinePartRange> ValidRange( Pipeline current, AllPipelines context, int index )
    {
        yield return MachinePartRange.Full();
    }
}

public struct RejectRule : IRule
{
    public IRule ValidateNext( MachinePart part, Pipeline current, AllPipelines context, int index ) => this;

    public IEnumerable<MachinePartRange> ValidRange( Pipeline current, AllPipelines context, int index )
    {
        yield break;
    }


}

public record struct ReferenceRule( string Ref ) : IRule
{
    public IRule ValidateNext( MachinePart part, Pipeline current, AllPipelines context, int index )
    {
        var pipeline = context[Ref];
        return pipeline.rules[0].ValidateNext( part, pipeline, context, 0 );
    }

    public IEnumerable<MachinePartRange> ValidRange( Pipeline current, AllPipelines context, int index )
    {
        var pipeline = context[Ref];
        return pipeline.rules[0].ValidRange( pipeline, context, 0 );
    }
}

public record struct LessThanRule( char attr, int value, IRule then ) : IRule
{
    public IRule ValidateNext( MachinePart part, Pipeline current, AllPipelines context, int index )
    {
        if ( part.Get( attr ) < value )
        {
            return then.ValidateNext( part, current, context, index );
        }
        else
        {
            return current.rules[index + 1].ValidateNext( part, current, context, index + 1 );
        }
    }

    public IEnumerable<MachinePartRange> ValidRange( Pipeline current, AllPipelines context, int index )
    {
        // Do the true path first:
        var lessThan = MachinePartRange.LessThan( attr, value );
        foreach ( var range in then.ValidRange( current, context, index ) )
        {
            yield return range.Intersect( lessThan );
        }

        // false path:
        var greaterThan = MachinePartRange.GreaterThan( attr, value - 1 );
        foreach ( var range in current.rules[index + 1].ValidRange( current, context, index + 1 ) )
        {
            yield return range.Intersect( greaterThan );
        }
    }
}

public record struct GreaterThanRule( char attr, int value, IRule then ) : IRule
{
    public IRule ValidateNext( MachinePart part, Pipeline current, AllPipelines context, int index )
    {
        if ( part.Get( attr ) > value )
        {
            return then.ValidateNext( part, current, context, index );
        }
        else
        {
            return current.rules[index + 1].ValidateNext( part, current, context, index + 1 );
        }
    }

    public IEnumerable<MachinePartRange> ValidRange( Pipeline current, AllPipelines context, int index )
    {
        // true path:
        var greaterThan = MachinePartRange.GreaterThan( attr, value );
        foreach ( var range in then.ValidRange( current, context, index ) )
        {
            yield return range.Intersect( greaterThan );
        }

        // false path:
        var lessThan = MachinePartRange.LessThan( attr, value + 1 );
        foreach ( var range in current.rules[index + 1].ValidRange( current, context, index + 1 ) )
        {
            yield return range.Intersect( lessThan );
        }
    }
}

public record struct LongRange( long low, long high )
{
    public static LongRange Empty() => new LongRange( 0, 0 );
    public static LongRange Full() => new LongRange( 1, 4000 );

    public static LongRange GreaterThan( long value ) => new LongRange( value + 1, 4000 );
    public static LongRange LessThan( long value ) => new LongRange( 1, value - 1 );

    public LongRange Intersect( LongRange other )
    {
        var low = System.Math.Max( this.low, other.low );
        var high = System.Math.Min( this.high, other.high );
        if ( low > high )
            throw new Exception( $"Invalid range ({low}, {high})" );
        return new LongRange( low, high );
    }

    public long Length => high - low + 1;
}

public record struct MachinePartRange( LongRange x, LongRange m, LongRange a, LongRange s )
{
    public static MachinePartRange Empty() => new MachinePartRange( LongRange.Empty(), LongRange.Empty(), LongRange.Empty(), LongRange.Empty() );
    public static MachinePartRange Full() => new MachinePartRange( LongRange.Full(), LongRange.Full(), LongRange.Full(), LongRange.Full() );

    public MachinePartRange Intersect( MachinePartRange other )
        => new MachinePartRange( x.Intersect( other.x ), m.Intersect( other.m ), a.Intersect( other.a ), s.Intersect( other.s ) );

    public long Score() => x.Length * m.Length * a.Length * s.Length;

    public static MachinePartRange GreaterThan( char attr, long value )
        => attr switch {
            'x' => new MachinePartRange( LongRange.GreaterThan( value ), LongRange.Full(), LongRange.Full(), LongRange.Full() ),
            'm' => new MachinePartRange( LongRange.Full(), LongRange.GreaterThan( value ), LongRange.Full(), LongRange.Full() ),
            'a' => new MachinePartRange( LongRange.Full(), LongRange.Full(), LongRange.GreaterThan( value ), LongRange.Full() ),
            's' => new MachinePartRange( LongRange.Full(), LongRange.Full(), LongRange.Full(), LongRange.GreaterThan( value ) ),
            _ => throw new ArgumentException( $"Unknown attribute {attr}" )
        };

    public static MachinePartRange LessThan( char attr, long value )
        => attr switch {
            'x' => new MachinePartRange( LongRange.LessThan( value ), LongRange.Full(), LongRange.Full(), LongRange.Full() ),
            'm' => new MachinePartRange( LongRange.Full(), LongRange.LessThan( value ), LongRange.Full(), LongRange.Full() ),
            'a' => new MachinePartRange( LongRange.Full(), LongRange.Full(), LongRange.LessThan( value ), LongRange.Full() ),
            's' => new MachinePartRange( LongRange.Full(), LongRange.Full(), LongRange.Full(), LongRange.LessThan( value ) ),
            _ => throw new ArgumentException( $"Unknown attribute {attr}" )
        };
}


public static partial class Helpers
{
    #region parsers
    public static readonly Parser<MachinePart> MachinePartParser =
        Parse.Char( '{' )
            .Then( _ => Parse.Chars( 'x', 'm', 'a', 's' )
                .Then( c => Parse.Char( '=' ).Select( _ => c ) )
                .Then( c => Parse.Number.Select( int.Parse ).Select( n => (c, n) ) )
                .DelimitedBy( Parse.Char( ',' ) )
            )
            .Select( attrs => attrs.ToDictionary( attr => attr.c, attr => attr.n ) )
            .Then( attrs => Parse.Char( '}' ).Select( _ => attrs ) )
            .Select( attrs => new MachinePart( attrs['x'], attrs['m'], attrs['a'], attrs['s'] ) );

    public static readonly Parser<AcceptRule> AcceptRuleParser =
        Parse.Char( 'A' ).Select( _ => new AcceptRule() );

    public static readonly Parser<RejectRule> RejectRuleParser =
        Parse.Char( 'R' ).Select( _ => new RejectRule() );

    public static readonly Parser<ReferenceRule> ReferenceRuleParser =
        Parse.Lower.AtLeastOnce().Text().Select( refName => new ReferenceRule( refName ) );

    public static readonly Parser<IRule> ComparisonRule =
        Parse.Chars( 'x', 'm', 'a', 's' )
            .Then( attr => Parse.Chars( '<', '>' ).Select( op => (attr, op) ) )
            .Then( item => Parse.Number.Select( int.Parse ).Select( n => (item.attr, item.op, n) ) )
            .Then( item => Parse.Char( ':' ).Select( _ => item ) )
            .Then( item => RuleParser.Select( then => (item.attr, item.op, item.n, then) ) )
            .Select( item => item.op switch {
                '<' => new LessThanRule( item.attr, item.n, item.then ) as IRule,
                '>' => new GreaterThanRule( item.attr, item.n, item.then ) as IRule,
                _ => throw new ArgumentException( $"Invalid comparison operator {item.op}" )
            } );

    public static readonly Parser<IRule> RuleParser =
        ComparisonRule
        .Or( ReferenceRuleParser.Select( r => r as IRule ) )
        .Or( RejectRuleParser.Select( r => r as IRule ) )
        .Or( AcceptRuleParser.Select( r => r as IRule ) );


    public static readonly Parser<Pipeline> PipelineParser =
        Parse.Letter.AtLeastOnce().Text()
            .Then( name => Parse.Char( '{' ).Select( _ => name ) )
            .Then( name => RuleParser.DelimitedBy( Parse.Char( ',' ) ).Select( rules => (name, rules) ) )
            .Then( item => Parse.Char( '}' ).Select( _ => new Pipeline( item.name, item.rules.ToList() ) ) );

    public static readonly Parser<AllPipelines> AllPipelinesParser =
        PipelineParser.DelimitedBy( Parse.Char( '\n' ).Once() )
            .Select( pipelines => pipelines.ToDictionary( p => p.Name, p => p ) )
            .Select( pipelines => new AllPipelines( pipelines ) );

    public static readonly Parser<(AllPipelines, IEnumerable<MachinePart>)> InputParser =
        AllPipelinesParser
            .Then( pipelines => Parse.Char( '\n' ).Repeat( 2 ).Select( _ => pipelines ) )
            .Then( pipelines => MachinePartParser.DelimitedBy( Parse.Char( '\n' ) ).Select( parts => (pipelines, parts) ) );

    public static (AllPipelines pipelines, IEnumerable<MachinePart> parts) ParseInput( string filename )
        => InputParser.Parse( string.Concat( filename.Stream().GetChars() ) );
    #endregion parsers
}
