using Sprache;
using AoC.Utils;
using AoC.Utils.LinearProgramming;
using MathNet.Numerics.LinearAlgebra;

namespace AoC.Y2025;

public sealed class Day10 : IPuzzle
{
    private IEnumerable<MachineWithLights> TestData =
        "2025/inputdata/day10_test.txt".GetLines().Select( Helpers.MachineWithLights.Parse ).ToList();
    private IEnumerable<MachineWithLights> Data =
        "2025/inputdata/day10.txt".GetLines().Select( Helpers.MachineWithLights.Parse ).ToList();

    public void Part1()
    {
        var data = TestData;
        var result = data.Skip( 1 ).Take( 1 ).Sum( m => m.ReachDesiredState() );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var data = Data;
        var result = data.Sum( m => m.ReachDesiredJoltage() );
        Console.WriteLine( ( long ) result );
    }
}

record struct IndicatorLights( List<bool> Lights )
{
    private static char ToChar( bool f ) => f ? '#' : '.';
    public override readonly string ToString() => $"[{string.Concat( Lights.Select( ToChar ) )}]";

    public static IndicatorLights AllOff( int length ) => new IndicatorLights( Enumerable.Range( 0, length ).Select( _ => false ).ToList() );

    public IndicatorLights Apply( ButtonSchematic schematic )
    {
        var newLights = Lights.Select( ( l, i ) => schematic.Buttons.Contains( i ) ? !l : l ).ToList();
        return new IndicatorLights( newLights );
    }

    public LinearFunctional ToFunctional() =>
        new LinearFunctional( Vector<double>.Build.DenseOfEnumerable( Lights.Select( l => l ? 1.0 : 0.0 ) ) );

    public bool DeepCompare( IndicatorLights? other )
    {
        if ( other is null ) return false;
        if ( other.Value.Lights.Count() != Lights.Count() ) return false;
        return other.Value.Lights.Zip( Lights ).All( p => p.First == p.Second );
    }

}
record struct ButtonSchematic( List<long> Buttons )
{
    public override readonly string ToString() => $"({string.Join( ',', Buttons )})";

    public Vector<double> ToCoefficientVector( int n )
    {
        var result = Enumerable.Range( 0, n ).Select( _ => 0.0 ).ToArray();
        Buttons.ForEach( i => result[i] = 1.0 );
        return Vector<double>.Build.DenseOfArray( result );
    }
}
record struct JoltageRequirement( List<long> Joltages )
{
    public override readonly string ToString() => $"{{{string.Join( ',', Joltages )}}}";

    public Vector<double> ToVector() =>
        Vector<double>.Build.DenseOfArray(
            [.. Joltages.Select( j => ( double ) j )]
        );

    public LinearFunctional ToFunctional() =>
        new LinearFunctional( ToVector() );
}
record struct MachineWithLights( IndicatorLights DesiredState, List<ButtonSchematic> Buttons, JoltageRequirement JRequirements )
{
    public override readonly string ToString() => $"{DesiredState} {string.Join( ' ', Buttons )} {JRequirements}";

    public static IEnumerable<IndicatorLights> ApplyTo( List<ButtonSchematic> buttonSchematics, IndicatorLights lights ) => buttonSchematics.Select( b => lights.Apply( b ) );

    public LinearFunctional NumPresses() =>
        LinearFunctional.Constant( 1.0, Buttons.Count );

    public LinearOperator ButtonsOperator( int dimension ) =>
        new LinearOperator( Matrix<double>.Build.DenseOfColumnVectors(
            Buttons.Select( but => but.ToCoefficientVector( dimension ) ).ToArray()
        ) );

    public double ReachDesiredState()
    {
        Console.WriteLine( this );
        // Assemble Simplex program
        var b = DesiredState.ToFunctional();
        var c = NumPresses();
        var I = LinearOperator.Identity( c.Dimensions() );
        var A = ButtonsOperator( b.Dimensions() );
        var simplex = LP<Simplex>.Minimize( c )
            .Given( A ^ b )
            .Given( I <= 1.0 )
            .AllInteger();
        Console.WriteLine( simplex.Problem.Tableu );
        Console.WriteLine( string.Join( ',', simplex.Problem.BasicVars ) );
        var result = simplex.Solve();
        Console.WriteLine( simplex.Problem.Tableu );
        Console.WriteLine( string.Join( ',', simplex.Problem.BasicVars ) );
        Console.WriteLine( $"Solution: {simplex.Problem.AllSolutionCoefficients()}" );
        Console.WriteLine( $"Objective is {simplex.CurrentObjective()}" );
        return result;

        //var initial = IndicatorLights.AllOff( DesiredState.Lights.Count() );
        //var buttons = Buttons;
        //var target = DesiredState;
        //return ShortestPath.BFS(
        //    initial,
        //    state => state.DeepCompare( target ),
        //    state => {
        //        return ApplyTo( buttons, state );
        //    }
        //);
    }

    public double ReachDesiredJoltage()
    {
        // Assemble Simplex program
        var b = JRequirements.ToFunctional();
        var A = ButtonsOperator( b.Dimensions() );
        var simplex = LP<Simplex>.Minimize( NumPresses() )
            .Given( A == b )
            .AllInteger();

        return simplex.Solve();
    }
}

static partial class Helpers
{
    public static readonly Parser<IndicatorLights> IndicatorLights =
        Parse.Char( '[' )
        .Then( _ => Parse.Char( '.' ).Select( _ => false ).Or( Parse.Char( '#' ).Select( _ => true ) ).AtLeastOnce() )
        .Then( cs => Parse.Char( ']' ).Select( _ => cs ) )
        .Select( cs => new IndicatorLights( cs.ToList() ) );

    public static readonly Parser<ButtonSchematic> ButtonSchematic =
        Parse.Char( '(' )
        .Then( _ => Long.DelimitedBy( Parse.Char( ',' ) ) )
        .Then( cs => Parse.Char( ')' ).Select( _ => cs ) )
        .Select( cs => new ButtonSchematic( cs.ToList() ) );

    public static readonly Parser<JoltageRequirement> JoltageRequirement =
        Parse.Char( '{' )
        .Then( _ => Long.DelimitedBy( Parse.Char( ',' ) ) )
        .Then( cs => Parse.Char( '}' ).Select( _ => cs ) )
        .Select( cs => new JoltageRequirement( cs.ToList() ) );

    public static readonly Parser<MachineWithLights> MachineWithLights =
        IndicatorLights.Then( lights => Parse.WhiteSpace.Select( _ => lights ) )
        .Then( lights => ButtonSchematic.DelimitedBy( Parse.WhiteSpace ).Select( buttons => (lights, buttons) ) )
        .Then( p => Parse.WhiteSpace.Select( _ => p ) )
        .Then( p => JoltageRequirement.Select( j => (p.lights, p.buttons, j) ) )
        .Select( t => new MachineWithLights( t.lights, t.buttons.ToList(), t.j ) );
}
