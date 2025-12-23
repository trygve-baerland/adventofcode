using MathNet.Numerics.LinearAlgebra;
using AoC.Utils;
using System.Security.Cryptography;
using System.Reflection.PortableExecutable;

namespace AoC.Utils.LinearProgramming;

public record Simplex : ILinearProgram<Simplex>
{
    public required Matrix<double> Tableu { get; set; }
    /// <summary>
    /// BasicVars is a collection of the column indices for the current BFS (Basic feasible solution)
    /// Their corresponding coefficients are found, I think, in the last column.
    /// We start with the BFS that we only use the newly introduced variables,
    /// which can be found in indices m, m+1,...,m+n
    /// </summary>
    public required int[] BasicVars { get; set; }
    public required int NumUnknowns { get; set; }
    public required int NumConstraints { get; set; }

    private List<int> integerUnknowns = [];
    public double M { get; set; } = 1000;
    private bool onPrimal = true;

    public double CurrentObjective() => -Tableu[0, Tableu.ColumnCount - 1];

    public IEnumerable<double> RowCoefficients( int row ) =>
        Tableu.Row( row ).Skip( 1 ).Take( Tableu.ColumnCount - 2 );
    public IEnumerable<double> CurrentObjectiveFunctional =>
        RowCoefficients( 0 );

    public IEnumerable<double> ColumnCoefficients( int col ) =>
        Tableu.Column( col ).Skip( 1 ).Take( NumConstraints );
    public IEnumerable<double> CurrentSolutionCoefficients =>
        ColumnCoefficients( NumConstraints + NumUnknowns + 1 );

    public Vector<double> CurrentSolution()
    {
        var result = Vector<double>.Build.Dense( NumUnknowns + NumConstraints, 0.0 );
        for ( int i = 0; i < BasicVars.Length; i++ )
        {
            result[BasicVars[i]] = Tableu[1 + i, Tableu.ColumnCount - 1];
        }
        return result.SubVector( 0, NumUnknowns );
    }

    public static Simplex Minimize( LinearFunctional functional )
    {
        var c = functional.Coefficients;
        var tab = Matrix<double>.Build.Dense( 1, 2 + c.Count, 0.0 );
        tab[0, 0] = 1.0;
        tab.SetSubMatrix( 0, 1, c.ToRowMatrix() );
        return new Simplex {
            Tableu = tab,
            BasicVars = [],
            NumUnknowns = c.Count,
            NumConstraints = 0
        };
    }
    public void AddConstraint( ILinearConstraint constraint )
    {
        switch ( constraint )
        {
            case LessThanConstraint lc:
                addConstraint( lc.A, lc.B, true, null, false, null );
                break;
            case EqualityConstraint ec:
                addConstraint( ec.A, ec.B, false, null, true, null );
                break;
            case IntegerConstraint ic:
                if ( ic.index < 0 || ic.index >= NumUnknowns )
                    throw new ArgumentException( $"Invalid index for unknown. {ic.index}" );
                integerUnknowns.Add( ic.index );
                break;
            case BinaryConstraint bc:
                addConstraint( bc.A, bc.B, true, -2.0, true, null );
                break;
            default:
                throw new ArgumentException( $"Invalid constraint type: {constraint.GetType()}" );
        }
    }

    private void addConstraint(
        Matrix<double> A,
        Vector<double> b,
        bool addSlacks,
        double? slackScale,
        bool addVars,
        double? varScale
    )
    {
        // Initial assertions
        var n = A.RowCount;
        var m = A.ColumnCount;
        // We allow inputting constraints only for the unknowns.
        if ( m == NumUnknowns )
        {
            m = Tableu.ColumnCount - 2;
        }

        if ( b.Count != n )
        {
            throw new ArgumentException( $"Mismatch in number of constraints and values: {n} != {b.Count}" );
        }
        if ( m != Tableu.ColumnCount - 2 )
        {
            throw new ArgumentException( "The given equality constraint doesn't match the problem:" +
                $" {m}(A.ColumnCount) != {NumConstraints + NumUnknowns}(number of unknowns and constraints thus far)" );
        }

        // Let's figure out what submatrices to add
        var numNewColumns = 0;
        if ( addSlacks ) numNewColumns += n;
        if ( addVars ) numNewColumns += n;

        // Let's initialize the new tableu:
        var newTab = Matrix<double>.Build.Dense(
            Tableu.RowCount + n,
            Tableu.ColumnCount + numNewColumns,
            0.0
        );
        // The first block is the same as before
        var oldB = Tableu.SubMatrix( 0, Tableu.RowCount, Tableu.ColumnCount - 1, 1 );
        var oldTab = Tableu.SubMatrix( 0, Tableu.RowCount, 0, Tableu.ColumnCount - 1 );
        newTab.SetSubMatrix( 0, 0, oldTab );
        newTab.SetSubMatrix( 0, Tableu.ColumnCount - 1 + numNewColumns, oldB );
        newTab.SetSubMatrix( Tableu.RowCount, Tableu.ColumnCount - 1 + numNewColumns, b.ToColumnMatrix() );
        // Set the constraint coefficients:
        newTab.SetSubMatrix( Tableu.RowCount, 1, A );

        // Update basic variables by extending with the newly introduced constraint variables
        var newBasics = Enumerable.Range(
            NumUnknowns, BasicVars.Length + numNewColumns
        ).ToArray();
        Array.Copy( BasicVars, 0, newBasics, 0, BasicVars.Length );
        BasicVars = newBasics;
        NumConstraints += n;
        // Add identity submatrices for slacks and variables:
        var I = Matrix<double>.Build.DenseIdentity( n );
        var rowOffset = Tableu.RowCount;
        var colOffset = Tableu.ColumnCount - 1;
        Tableu = newTab;

        if ( addVars )
        {
            Tableu.SetSubMatrix(
                rowOffset,
                colOffset,
                (varScale ?? 1.0) * I
            );
            // Finally, we penalize these heavily, so they
            // won't show up in the solution:
            //Tableu.SetSubMatrix( 0, m + 1, Vector<double>.Build.Dense( n, M ).ToRowMatrix() );
            Tableu.SetSubMatrix(
                0, colOffset,
                Vector<double>.Build.Dense( n, M ).ToRowMatrix()
            );

            // We perform initialization
            Initialize( rowOffset, colOffset, n );
            colOffset += n;
        }
        if ( addSlacks )
        {
            Tableu.SetSubMatrix(
                rowOffset,
                colOffset,
                (slackScale ?? 1.0) * I
            );
            colOffset += n;
        }


    }

    private bool cutNonIntegerUnknowns()
    {
        if ( integerUnknowns.Count < 1 ) return false;
        var nonIntegersIndices = integerUnknowns
            .Select( index => BasicVars.Index()
                .FirstOrDefault( item => item.Item == index, (Index: -1, Item: 0) ).Index
            )
            .Where( index => index >= 0 && !Tableu[1 + index, Tableu.ColumnCount - 1].IsInteger() )
            .ToList();

        if ( nonIntegersIndices.Count == 0 ) return false;

        var ec = CreateIntegerConstraintsFor( nonIntegersIndices );
        AddConstraint( ec );
        onPrimal = false;
        return true;
    }

    // Run through inverse iteration n times to get initial tableau
    public void Initialize( int row, int col, int count )
    {
        foreach ( var i in Enumerable.Range( 0, count ) )
        {
            Tableu.Pivot( row + i, col + i );
        }
    }

    private int? getPivotColumn()
    {
        return CurrentObjectiveFunctional
            .MinIndex<double, double>( f => f < -1E-8 ? f : null ) + 1;
    }

    private int? getDualPivotRow()
    {
        return CurrentSolutionCoefficients
            .MinIndex<double, double>( f => f < -1E-8 ? f : null ) + 1;
    }
    private int? getPivotRow( int col )
    {
        return CurrentSolutionCoefficients.Zip(
            ColumnCoefficients( col )
        ).MinIndex<(double, double), double>( p => {
            if ( p.Item2 < 1E-8 ) return null;
            return p.Item1 / p.Item2;
        } ) + 1;
    }

    private int? getDualPivotCol( int row )
    {
        return CurrentObjectiveFunctional.Zip(
            RowCoefficients( row )
        ).MinIndex<(double, double), double>( p => {
            if ( p.Item2 > -1E-8 ) return null;
            return p.Item1 / p.Item2;
        } ) + 1;
    }

    private (int row, int col)? getPivotIndices()
    {
        if ( onPrimal )
        {
            var col = getPivotColumn();
            if ( col is null ) return null;
            var row = getPivotRow( col.Value );
            if ( row is null ) throw new ArgumentException( "Pivot row is null, for some reason" );
            return (row.Value, col.Value);
        }
        var dRow = getDualPivotRow();
        if ( dRow is null ) return null;
        var dCol = getDualPivotCol( dRow.Value );
        if ( dCol is null ) throw new ArgumentException( "Dual pivot column is null, for some reason." );
        return (dRow.Value, dCol.Value);
    }

    public bool Iterate()
    {
        Console.WriteLine( Tableu );
        var pivot = getPivotIndices();
        if ( pivot is null )
        {
            if ( onPrimal )
            {
                // At this point, we've optimized the relaxed problem.
                // I.e., we've found an optimum without necessarily all variables being integer
                // So let's cut where we need
                return !cutNonIntegerUnknowns();
            }
            else
            {
                // Switch back to primal solve:
                onPrimal = true;
                return false;
            }
        }
        // Do iteration
        Tableu.Pivot( pivot.Value.row, pivot.Value.col );

        // Update basic vars:
        BasicVars[pivot.Value.row - 1] = pivot.Value.col - 1;
        // return (well, duh)
        return false;
    }

    private LessThanConstraint CreateIntegerConstraintsFor( IEnumerable<int> rowIndices )
    {
        var mat = Matrix<double>.Build.DenseOfRowVectors(
            rowIndices.Select( ri => Tableu.Row( ri + 1 ).Map( v => double.Floor( v ) - v ) )
        );
        var A = mat.SubMatrix( 0, mat.RowCount, 1, mat.ColumnCount - 2 );
        var b = mat.Column( mat.ColumnCount - 1 );
        return new LessThanConstraint( A, b );
    }
}
