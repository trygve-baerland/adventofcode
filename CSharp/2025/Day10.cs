using Sprache;
using AoC.Utils;
using MathNet.Numerics.LinearAlgebra;
using LpSolveDotNet;
using MathNet.Numerics;

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
        var result = data.Sum( m => m.ReachDesiredState() );
        Console.WriteLine( result );
    }

    public void Part2()
    {
        LpSolve.Init();

        var data = Data;
        var result = data.Skip( 10 ).Take( 1 ).Select( ( m, i ) => {
            var result = m.ReachDesiredJoltage();
            var oldRes = m.OldSolve();
            return (i, result, oldRes);
        } ).Where( tup => tup.result != tup.oldRes )
        .Sum( tup => {
            Console.WriteLine( $"{tup.i}: {tup.result} || {tup.oldRes}" );
            return tup.result;
        } );
        Console.WriteLine( result );
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
            Joltages.Select( j => ( double ) j ).ToArray()
        );
}
record struct MachineWithLights( IndicatorLights DesiredState, List<ButtonSchematic> Buttons, JoltageRequirement JRequirements )
{
    public override readonly string ToString() => $"{DesiredState} {string.Join( ' ', Buttons )} {JRequirements}";

    public static IEnumerable<IndicatorLights> ApplyTo( List<ButtonSchematic> buttonSchematics, IndicatorLights lights ) => buttonSchematics.Select( b => lights.Apply( b ) );

    public long ReachDesiredState()
    {
        var initial = IndicatorLights.AllOff( DesiredState.Lights.Count() );
        var buttons = Buttons;
        var target = DesiredState;
        return ShortestPath.BFS(
            initial,
            state => state.DeepCompare( target ),
            state => {
                return ApplyTo( buttons, state );
            }
        );
    }

    public long ReachDesiredJoltage()
    {
        // Console.WriteLine( $"Solving for {this}" );
        // Assemble Simplex program
        var b = JRequirements.ToVector();
        var A = Matrix<double>.Build.DenseOfColumnVectors(
            Buttons.Select( but => but.ToCoefficientVector( b.Count ) ).ToArray()
        );
        var c = Vector<double>.Build.Dense( A.ColumnCount, 1.0 );
        var simplex = SimplexProblem.FromCoeffs( A, b, c );
        Console.WriteLine( simplex.Tableu );

        var count = 0;
        while ( !simplex.Iterate() )
        {
            // Console.WriteLine( simplex.Tableu );
            // Console.WriteLine( $"BasicVars: {string.Join( ',', simplex.BasicVars )}" );
            count++;
        }
        Console.WriteLine( simplex.Tableu );
        // Console.WriteLine( $"Solved in {count} iterations" );
        Console.WriteLine( $"Solution is {simplex.CurrentSolution}" );
        Console.WriteLine( $"A*x = {A.Multiply( simplex.CurrentSolution )}" );
        // Console.WriteLine( $"Objective: {simplex.CurrentObjective}" );
        return simplex.CurrentObjective;
    }

    public readonly long OldSolve()
    {
        var b = JRequirements.ToVector();
        var A = Matrix<double>.Build.DenseOfColumnVectors(
            Buttons.Select( but => but.ToCoefficientVector( b.Count ) ).ToArray()
        );
        var c = Vector<double>.Build.Dense( A.ColumnCount, 1.0 );

        var lp = LpSolve.make_lp( b.Count, A.ColumnCount );
        lp.set_minim();
        const double Ignored = 0;
        double[] cArr = new double[c.Count + 1];
        cArr[0] = Ignored;
        Array.Copy( c.ToArray(), 0, cArr, 1, c.Count );
        lp.set_obj_fn( cArr );
        lp.set_add_rowmode( true );
        for ( int i = 0; i < A.RowCount; i++ )
        {
            double[] aRow = new double[A.ColumnCount + 1];
            aRow[0] = Ignored;
            Array.Copy( A.Row( i ).ToArray(), 0, aRow, 1, A.ColumnCount );
            lp.add_constraint( aRow, lpsolve_constr_types.EQ, b[i] );
        }
        lp.set_add_rowmode( false );
        for ( var i = 1; i <= A.ColumnCount; i++ )
        {
            lp.set_int( i, true );
        }
        lp.set_verbose( lpsolve_verbosity.IMPORTANT );
        var result = lp.solve();
        if ( result == lpsolve_return.OPTIMAL )
        {
            var rounded = ( long ) lp.get_objective().Round( 0 );
            return rounded;
        }
        else
        {
            throw new Exception( "Coulnd't optimize" );
        }
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
