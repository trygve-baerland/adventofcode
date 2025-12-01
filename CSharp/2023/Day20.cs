using System.Text;
using AoC.Utils;
using Sprache;

namespace AoC.Y2023;

public sealed class Day20 : IPuzzle
{
    public ModuleResolver TestModule() => Helpers.ModuleResolverParser.Parse( string.Concat( "2023/inputdata/day20_test.txt".Stream().GetChars() ) );
    public ModuleResolver ActualModule() => Helpers.ModuleResolverParser.Parse( string.Concat( "2023/inputdata/day20.txt".Stream().GetChars() ) );
    public void Part1()
    {
        var resolver = ActualModule();
        var buttonPresses = (counter: new SignalCounter( 0, 0 ), resolver).FixPoint( item => {
            var newcounter = item.resolver.PressButton();
            return (item.counter + newcounter, resolver);
        } )
        .Take( 1001 )
        .TakeWhile( item => !item.resolver.AtDefault() || item.counter.Score() == 0, includeLast: true )
        .Last();
        var counter = buttonPresses.counter;
        Console.WriteLine( counter.Score() );
    }

    public void Part2()
    {
        var resolver = ActualModule();
        var result = new List<string>() { "nx", "sp", "cc", "jq" }
            .Select( end => {
                resolver.Reset();
                return (resolver, reached: false)
                    .FixPoint( item => {
                        var reached = item.resolver.PressFor( end, Signal.Low );
                        if ( reached )
                        {
                            return (item.resolver, true);
                        }
                        return (item.resolver, false);
                    } )
                    .TakeWhile( item => !item.reached )
                    .Count();
            } )
            .Aggregate( seed: 1L, ( acc, item ) => Utils.Math.LCM( acc, item ) );
        var otherResult = new List<int>() { 4013, 4001, 3911, 3851 }
        .Aggregate( seed: 1L, ( acc, item ) => Utils.Math.LCM( acc, item ) );
        Console.WriteLine( result );
    }
}

public enum Signal : byte
{
    Low = 0,
    High = 1
}

public record struct SignalCounter( long low, long high )
{
    public long Score() => low * high;
    public SignalCounter Add( Signal s ) => s switch {
        Signal.Low => new SignalCounter( low + 1, high ),
        Signal.High => new SignalCounter( low, high + 1 ),
        _ => throw new ArgumentException( "Invalid signal" )
    };

    public static SignalCounter operator *( long lhs, SignalCounter rhs ) => new SignalCounter( lhs * rhs.low, lhs * rhs.high );

    public static SignalCounter operator +( SignalCounter lhs, SignalCounter other ) => new SignalCounter( lhs.low + other.low, lhs.high + other.high );
}

public interface ISignalModule
{
    public string name { get; }
    public List<string> Children { get; }
    public void AddChild( string name ) => Children.Add( name );

    public void BuildString( StringBuilder sb );

    public Signal? ReceiveSignal( Signal s, string from );
    public void Reset();

    public bool AtDefault();
}

public record class FlipFlopModule( string name ) : ISignalModule
{
    public bool IsSet { get; private set; } = false;
    public List<string> Children { get; } = new();
    public void Reset() => IsSet = false;

    public bool AtDefault() => !IsSet;
    public void BuildString( StringBuilder sb )
    {
        var on = IsSet ? "on" : "off";
        sb.AppendLine( $"%{name}({on}) -> {string.Join( ',', Children )}" );
    }

    public Signal? ReceiveSignal( Signal s, string from )
    {
        if ( s == Signal.High )
        {
            return null;
        }
        // We received a low pulse:

        IsSet = !IsSet;
        return IsSet ? Signal.High : Signal.Low;
    }
}

public record class BroadcasterModule( string name ) : ISignalModule
{
    public List<string> Children { get; } = new();

    public void BuildString( StringBuilder sb )
    {
        sb.AppendLine( $"{name} -> {string.Join( ',', Children )}" );
    }

    public Signal? ReceiveSignal( Signal s, string from ) => s;

    public bool AtDefault() => true;
    public void Reset() { }
}

public record class ConjunctionModule( string name ) : ISignalModule
{
    public List<string> Children { get; } = new();
    public Dictionary<string, Signal> Inputs { get; } = new();

    public void BuildString( StringBuilder sb )
    {
        var lastInput = string.Join( ',', Inputs.Select( kvp => LastInputAsString( kvp.Key, kvp.Value ) ) );
        sb.AppendLine( $"&{name}({lastInput}) -> {string.Join( ',', Children )}" );
    }

    public bool AtDefault() => Inputs.Values.All( v => v == Signal.Low );

    private static string LastInputAsString( string name, Signal s ) => $"{name}({s})";

    public Signal? ReceiveSignal( Signal s, string from )
    {
        Inputs[from] = s;
        if ( Inputs.Values.Any( v => v == Signal.Low ) )
        {
            return Signal.High;
        }
        return Signal.Low;
    }

    public void Reset()
    {
        foreach ( var key in Inputs.Keys )
        {
            Inputs[key] = Signal.Low;
        }
    }
}

public record class ReferenceModule( string name ) : ISignalModule
{
    public List<string> Children { get; } = new();

    public void BuildString( StringBuilder sb )
    {
        sb.AppendLine( name );
    }

    public Signal? ReceiveSignal( Signal s, string from ) => null;

    public bool AtDefault() => true;
    public void Reset() { }

}

public class ModuleResolver
{
    public Dictionary<string, ISignalModule> Modules { get; } = new();
    public ModuleResolver( Dictionary<string, (ISignalModule module, IEnumerable<ReferenceModule> children)> modules )
    {
        foreach ( var (name, (module, children)) in modules )
        {
            foreach ( var child in children )
            {
                module.AddChild( child.name );
                if ( !Modules.ContainsKey( child.name ) )
                {
                    Modules.Add( child.name, child );
                }
            }
            Modules[name] = module;
        }

        // Resolve parents of Conjunctions:
        foreach ( var (name, (module, children)) in modules )
        {
            foreach ( var childName in Modules[name].Children )
            {
                if ( Modules[childName] is ConjunctionModule conjunction )
                {
                    conjunction.Inputs.Add( name, Signal.Low );
                }
            }
        }
    }

    public bool AtDefault() => Modules.Values.All( m => m.AtDefault() );
    public SignalCounter PressButton()
    {
        // We are doing this as a BFS
        var counter = new SignalCounter( 1, 0 );
        var queue = new Queue<(ISignalModule module, (Signal signal, string from))>();
        queue.Enqueue( (Modules["broadcaster"]!, (Signal.Low, "button")) );
        while ( queue.Count > 0 )
        {
            var (module, (signal, from)) = queue.Dequeue();
            // Add to counter:
            var newSignal = module.ReceiveSignal( signal, from );
            // This signal should be broadcasted to all children:
            if ( newSignal.HasValue )
            {
                foreach ( var childName in module.Children )
                {
                    var child = Modules[childName];
                    queue.Enqueue( (child, (newSignal.Value, module.name)) );
                    counter = counter.Add( newSignal.Value );
                }
            }

        }
        return counter;
    }

    public bool PressFor( string end, Signal endSignal )
    {
        var queue = new Queue<(ISignalModule module, (Signal signal, string from))>();
        var found = false;
        queue.Enqueue( (Modules["broadcaster"]!, (Signal.Low, "button")) );
        while ( queue.Count > 0 )
        {
            var (module, (signal, from)) = queue.Dequeue();
            if ( module.name == end && signal == endSignal )
            {
                found = true;
            }
            var newSignal = module.ReceiveSignal( signal, from );
            // This signal should be broadcasted to all children:
            if ( newSignal.HasValue )
            {
                foreach ( var childName in module.Children )
                {
                    var child = Modules[childName];
                    queue.Enqueue( (child, (newSignal.Value, module.name)) );
                }
            }
        }
        // Never reached where we wanted to go:
        return found;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach ( var module in Modules.Values )
        {
            module.BuildString( builder );
        }
        return builder.ToString();
    }

    public void Reset()
    {
        foreach ( var module in Modules.Values )
        {
            module.Reset();
        }
    }
}


public static partial class Helpers
{
    #region parsers
    public static readonly Parser<string> ModuleIdentifier =
        Parse.Lower.AtLeastOnce().Text();

    public static readonly Parser<FlipFlopModule> FlipFlopModule =
        Parse.Char( '%' )
            .Then( _ => ModuleIdentifier )
            .Select( name => new FlipFlopModule( name ) );

    public static readonly Parser<BroadcasterModule> BroadcasterModule =
        ModuleIdentifier
            .Select( name => new BroadcasterModule( name ) );

    public static readonly Parser<ConjunctionModule> ConjunctionModule =
        Parse.Char( '&' )
            .Then( _ => ModuleIdentifier )
            .Select( name => new ConjunctionModule( name ) );

    public static readonly Parser<ReferenceModule> ReferenceModule =
        ModuleIdentifier
            .Select( name => new ReferenceModule( name ) );

    public static readonly Parser<ISignalModule> SignalModule =
        FlipFlopModule.Select( m => m as ISignalModule )
            .Or( ConjunctionModule.Select( m => m as ISignalModule ) )
            .Or( BroadcasterModule.Select( m => m as ISignalModule ) );

    public static readonly Parser<(ISignalModule module, IEnumerable<ReferenceModule> children)> ModuleWithChildren =
        SignalModule
            .Then( m => Parse.String( " -> " ).Select( _ => m ) )
            .Then( m => ReferenceModule.DelimitedBy( Parse.Char( ',' ).Token() ).Select( c => (m, c) ) );

    public static readonly Parser<ModuleResolver> ModuleResolverParser =
        ModuleWithChildren
            .DelimitedBy( Parse.Char( '\n' ).AtLeastOnce() )
            .Select( items => items.ToDictionary( item => item.module.name ) )
            .Select( modules => new ModuleResolver( modules ) );
    #endregion parsers

}
