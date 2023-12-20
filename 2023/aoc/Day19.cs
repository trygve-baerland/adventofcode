using Utils;
using Sprache;

namespace AoC;

public sealed class Day19 : IPuzzle
{
    public (IRule rule, IEnumerable<MachinePart> parts) TestInput() => Helpers.ParseInput( "inputdata/day19_test.txt" );
    public (IRule rule, IEnumerable<MachinePart> parts) ActualInput() => Helpers.ParseInput( "inputdata/day19.txt" );

    public void Part1()
    {
        var (rule, parts) = ActualInput();
        var result = parts.Where( rule.Validate ).Sum( part => part.Score() );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var (rule, parts) = ActualInput();
        var result = rule.ValidRange().Select( range => range.Score() ).Sum();
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

public record struct RuleResolver( Dictionary<string, IRule> rules )
{
    public IRule this[string name] => rules[name];

    public IRule Resolve( IRule rule ) =>
        rule switch {
            AcceptRule r => r,
            RejectRule r => r,
            ReferenceRule r => Resolve( r ),
            LessThanRule r => Resolve( r ),
            GreaterThanRule r => Resolve( r ),
            _ => throw new ArgumentException( $"Unknown rule type {rule.GetType()}" )
        };
    public IRule Resolve( ReferenceRule rule ) => Resolve( rule.Ref );

    public IRule Resolve( LessThanRule rule ) =>
        new LessThanRule( rule.attr, rule.value, Resolve( rule.yesRule ), Resolve( rule.noRule ) );

    public IRule Resolve( GreaterThanRule rule ) =>
        new GreaterThanRule( rule.attr, rule.value, Resolve( rule.yesRule ), Resolve( rule.noRule ) );

    public IRule Resolve( string name ) => Resolve( rules[name] );
}

public interface IRule
{
    public bool Validate( MachinePart part );
    public IEnumerable<MachinePartRange> ValidRange();

}

public struct AcceptRule : IRule
{
    public bool Validate( MachinePart part ) => true;

    public IEnumerable<MachinePartRange> ValidRange()
    {
        yield return MachinePartRange.Full();
    }
}

public struct RejectRule : IRule
{
    public bool Validate( MachinePart part ) => false;

    public IEnumerable<MachinePartRange> ValidRange()
    {
        yield break;
    }

}

public record struct ReferenceRule( string Ref ) : IRule
{
    public bool Validate( MachinePart part )
    {
        throw new Exception( $"Cannot validate reference '{Ref}'" );
    }

    public IEnumerable<MachinePartRange> ValidRange()
    {
        throw new Exception( $"Cannot validate reference '{Ref}'" );
    }
}

public record struct LessThanRule( char attr, int value, IRule yesRule, IRule noRule ) : IRule
{
    public bool Validate( MachinePart part ) =>
        (part.Get( attr ) < value ? yesRule : noRule).Validate( part );

    public IEnumerable<MachinePartRange> ValidRange()
    {
        // Do the true path first:
        var lessThan = MachinePartRange.LessThan( attr, value );
        foreach ( var range in yesRule.ValidRange() )
        {
            yield return range.Intersect( lessThan );
        }

        // false path:
        var greaterThan = MachinePartRange.GreaterThan( attr, value - 1 );
        foreach ( var range in noRule.ValidRange() )
        {
            yield return range.Intersect( greaterThan );
        }
    }
}

public record struct GreaterThanRule( char attr, int value, IRule yesRule, IRule noRule ) : IRule
{
    public bool Validate( MachinePart part ) =>
        (part.Get( attr ) > value ? yesRule : noRule).Validate( part );

    public IEnumerable<MachinePartRange> ValidRange()
    {
        // true path:
        var greaterThan = MachinePartRange.GreaterThan( attr, value );
        foreach ( var range in yesRule.ValidRange() )
        {
            yield return range.Intersect( greaterThan );
        }

        // false path:
        var lessThan = MachinePartRange.LessThan( attr, value + 1 );
        foreach ( var range in noRule.ValidRange() )
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
            .Then( item => RuleParser.Select( yes => (item.attr, item.op, item.n, yes) ) )
            .Then( item => Parse.Char( ',' ).Select( _ => item ) )
            .Then( item => RuleParser.Select( no => (item.attr, item.op, item.n, item.yes, no) ) )
            .Select( item => item.op switch {
                '<' => new LessThanRule( item.attr, item.n, item.yes, item.no ) as IRule,
                '>' => new GreaterThanRule( item.attr, item.n, item.yes, item.no ) as IRule,
                _ => throw new ArgumentException( $"Invalid comparison operator {item.op}" )
            } );

    public static readonly Parser<IRule> RuleParser =
        ComparisonRule
        .Or( ReferenceRuleParser.Select( r => r as IRule ) )
        .Or( RejectRuleParser.Select( r => r as IRule ) )
        .Or( AcceptRuleParser.Select( r => r as IRule ) );


    public static readonly Parser<(string name, IRule rule)> PipelineParser =
        Parse.Letter.AtLeastOnce().Text()
            .Then( name => Parse.Char( '{' ).Select( _ => name ) )
            .Then( name => RuleParser.Select( rule => (name, rule) ) )
            .Then( item => Parse.Char( '}' ).Select( _ => item ) );

    public static readonly Parser<RuleResolver> RuleResolverParser =
        PipelineParser.DelimitedBy( Parse.Char( '\n' ).Once() )
            .Select( pipelines => pipelines.ToDictionary( p => p.name, p => p.rule ) )
            .Select( pipelines => new RuleResolver( pipelines ) );

    public static readonly Parser<(IRule, IEnumerable<MachinePart>)> InputParser =
        RuleResolverParser.Select( resolver => resolver.Resolve( "in" ) )
            .Then( rule => Parse.Char( '\n' ).Repeat( 2 ).Select( _ => rule ) )
            .Then( rule => MachinePartParser.DelimitedBy( Parse.Char( '\n' ) ).Select( parts => (rule, parts) ) );

    public static (IRule rule, IEnumerable<MachinePart> parts) ParseInput( string filename )
        => InputParser.Parse( string.Concat( filename.Stream().GetChars() ) );
    #endregion parsers
}
