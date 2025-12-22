using System.Security.Principal;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace AoC.Utils;

public record SimplexProblem
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
    private static double M = 1000;
    private bool onPrimal = true;

    public long CurrentObjective => ( long ) -Tableu[0, Tableu.ColumnCount - 1].Round( 0 );

    public IEnumerable<double> RowCoefficients( int row ) =>
        Tableu.Row( row ).Skip( 1 ).Take( NumUnknowns + NumConstraints );
    public IEnumerable<double> CurrentObjectiveFunctional =>
        RowCoefficients( 0 );

    public IEnumerable<double> ColumnCoefficients( int col ) =>
        Tableu.Column( col ).Skip( 1 ).Take( NumConstraints );
    public IEnumerable<double> CurrentSolutionCoefficients =>
        ColumnCoefficients( NumConstraints + NumUnknowns + 1 );

    public Vector<double> CurrentSolution
    {
        get {
            var result = Vector<double>.Build.Dense( NumUnknowns + NumConstraints, 0.0 );
            for ( int i = 0; i < BasicVars.Length; i++ )
            {
                result[BasicVars[i]] = Tableu[1 + i, Tableu.ColumnCount - 1];
            }
            return result.SubVector( 0, NumUnknowns );
        }
    }
    public static SimplexProblem FromCoeffs( Matrix<double> A, Vector<double> b, Vector<double> c )
    {
        // initial assertions:
        var n = A.RowCount;
        var m = A.ColumnCount;
        if ( b.Count != n ) throw new ArgumentException( "Mismatch in number of constraints." );
        if ( c.Count != m ) throw new ArgumentException( "Mismatch in problem domain" );

        var tab = Matrix<double>.Build.Dense( 1, 2 + c.Count, 0.0 );
        // Set up tableau matrix
        tab[0, 0] = 1.0;
        tab.SetSubMatrix( 0, 1, c.ToRowMatrix() );

        //tab.SetSubMatrix( 1, m + n + 1, b.ToColumnMatrix() );
        // Set up M-constraints:
        // tab.SetSubMatrix( 0, m + 1, Vector<double>.Build.Dense( n, M ).ToRowMatrix() );
        // The current value for the objective functional will be updated after the Initialize call.
        //tab[0, tab.ColumnCount - 1] = M * b.Sum();

        var simplex = new SimplexProblem {
            Tableu = tab,

            BasicVars = [],
            NumUnknowns = m,
            NumConstraints = 0
        };
        simplex.AddEqualityConstraints( A, b );
        return simplex;
    }

    public void AddInequalityConstraints( Matrix<double> A, Vector<double> b )
    {
        // Initial assertions
        var n = A.RowCount;
        var m = A.ColumnCount;

        if ( b.Count != n )
        {
            throw new ArgumentException( $"Mismatch in number of constraints and values: {n} != {b.Count}" );
        }

        if ( m != NumConstraints + NumUnknowns )
        {
            throw new ArgumentException( "The given equality constraint doesn't match the problem:" +
                $" {m}(A.ColumnCount) != {NumConstraints + NumUnknowns}(number of unknowns and constraints thus far)" );
        }
        // Initialize new matrix
        var newTab = Matrix<double>.Build.Dense( Tableu.RowCount + n, Tableu.ColumnCount + n, 0.0 );
        // The first block is the same as before
        var oldB = Tableu.SubMatrix( 0, Tableu.RowCount, Tableu.ColumnCount - 1, 1 );
        var oldTab = Tableu.SubMatrix( 0, Tableu.RowCount, 0, Tableu.ColumnCount - 1 );
        newTab.SetSubMatrix( 0, 0, oldTab );
        newTab.SetSubMatrix( 0, n + m + 1, oldB );
        newTab.SetSubMatrix( Tableu.RowCount, n + m + 1, b.ToColumnMatrix() );
        // add in the new constraints:
        newTab.SetSubMatrix( 1 + NumConstraints, 1, A );
        // add in identity for the new constraint variables
        newTab.SetSubMatrix( 1 + NumConstraints, m + 1, Matrix<double>.Build.DenseIdentity( n ) );

        Tableu = newTab;
        // Update other state variables;
        NumConstraints += n;
        // Update basic variables by extending with the newly introduced constraint variables
        var newBasics = Enumerable.Range( NumUnknowns, BasicVars.Length + n ).ToArray();
        //var newBasics = new int[BasicVars.Length + n];
        Array.Copy( BasicVars, 0, newBasics, 0, BasicVars.Length );
        BasicVars = newBasics;

    }

    public void AddEqualityConstraints( Matrix<double> A, Vector<double> b )
    {
        AddInequalityConstraints( A, b );
        var n = A.RowCount;
        var m = A.ColumnCount;
        // Set up M-values for the new constraints:
        Tableu.SetSubMatrix( 0, m + 1, Vector<double>.Build.Dense( n, M ).ToRowMatrix() );
        // Tableu[0, Tableu.ColumnCount - 1] += M * b.Sum();

        // Finally we pivot on the new constraints
        var oldN = Tableu.RowCount - n;
        Initialize( Enumerable.Range( oldN, n ), NumUnknowns );

    }

    // Run through inverse iteration n times to get initial tableau
    public void Initialize( IEnumerable<int> variables, int offset )
    {
        foreach ( var i in variables )
        {
            Pivot( Tableu, i, offset + i );
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
        var pivot = getPivotIndices();
        if ( pivot is null )
        {
            if ( onPrimal )
            {
                // At this point, we've optimized the relaxed problem.
                // I.e., we've found an optimum without necessarily all variables being integer
                // So, let's find all basic variables that are non-integer:
                var nonIntegersIndices = Tableu.SubMatrix( 1, NumConstraints, Tableu.ColumnCount - 1, 1 )
                    .ToRowMajorArray()
                    .Select( ( v, i ) => (v, i) )
                    .Where( p => !p.v.IsInteger() && BasicVars[p.i] < NumUnknowns )
                    .Select( p => p.i )
                    .ToList();
                if ( nonIntegersIndices.Count > 0 )
                {
                    var (A, b) = CreateIntegerConstraintsFor( nonIntegersIndices );
                    AddInequalityConstraints( A, b );
                    onPrimal = false;
                    return false;
                }
                // We're done
                return true;
            }
            else
            {
                // Switch back to primal solve:
                onPrimal = true;
                return false;
            }
        }
        // Do iteration
        Pivot( Tableu, pivot.Value.row, pivot.Value.col );

        // Update basic vars:
        BasicVars[pivot.Value.row - 1] = pivot.Value.col - 1;
        // return (well, duh)
        return false;
    }

    private (Matrix<double> A, Vector<double> b) CreateIntegerConstraintsFor( IEnumerable<int> rowIndices )
    {
        var mat = Matrix<double>.Build.DenseOfRowVectors(
            rowIndices.Select( ri => Tableu.Row( ri + 1 ).Map( v => double.Floor( v ) - v ) )
        );
        var A = mat.SubMatrix( 0, mat.RowCount, 1, mat.ColumnCount - 2 );
        var b = mat.Column( mat.ColumnCount - 1 );
        return (A, b);
    }

    private static void ScaleRow( Matrix<double> mat, int row, double scale )
    {
        mat.SetRow( row, mat.Row( row ) * scale );
    }

    private static void RowCombine( Matrix<double> mat, int row1, int row2, double factor )
    {
        mat.SetRow( row1, mat.Row( row1 ) + mat.Row( row2 ) * factor );
    }

    private static void Pivot( Matrix<double> mat, int row, int col )
    {
        if ( double.Abs( mat[row, col] ) < 1E-8 ) throw new Exception( "Cannot pivot around 0" );
        ScaleRow( mat, row, 1.0 / mat[row, col] );
        // Combine every other row
        for ( int i = 0; i < mat.RowCount; i++ )
        {
            if ( i != row )
            {
                RowCombine( mat, i, row, -mat[i, col] );
            }
        }
    }
}
