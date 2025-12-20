using System.Net;
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
    public int numIterations = 100;
    private int currentIteration = 0;
    private static double M = 1000;
    private bool addedIntegerConstraints = false;

    public long CurrentObjective => ( long ) -Tableu[0, Tableu.ColumnCount - 1].Round( 0 );

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
        Tableu.SetSubMatrix( 0, Tableu.ColumnCount - (m - 1), Vector<double>.Build.Dense( n, M ).ToRowMatrix() );
        // Tableu[0, Tableu.ColumnCount - 1] += M * b.Sum();

        // Finally we pivot on the new constraints
        var oldN = Tableu.RowCount - n;
        Initialize( Enumerable.Range( oldN, n ), NumUnknowns );

    }

    // Run through inverse iteration n times to get initial tableau
    public void Initialize( IEnumerable<int> variables, int offset )
    {
        //Console.WriteLine( Tableu );
        foreach ( var i in variables )
        {
            // Console.WriteLine( $"Pivoting around: {i}, {offset + i}" );
            Pivot( Tableu, i, offset + i );
        }
    }

    private int? GetPivotColumn()
    {
        var objDiff = Tableu.SubMatrix( 0, 1, 1, NumUnknowns + NumConstraints ).ToRowMajorArray();
        if ( objDiff.All( f => f > -1E-8 ) )
        {
            return null;
        }
        // Get index of min
        var idx = 0;
        var target = double.MaxValue;
        for ( var i = 0; i < objDiff.Length; i++ )
        {
            if ( objDiff[i] < target )
            {
                target = objDiff[i];
                idx = i;
            }
        }
        return idx + 1;
    }

    private static bool SameSign( double a, double b )
    {
        // Some edge cases we want to short circuit
        // To be cleaned up
        if ( (double.Abs( a ) < 1E-8 && b < 1E-8) ||
            (double.Abs( b ) < 1E-8 && a < 1E-8) )
        {
            return false;
        }
        // This evaluationg doesn't work when a or b is close to 0.
        //return double.Sign( a ) != double.Sign( b );
        var d1 = double.Abs( b - a );
        var d2 = double.Abs( double.Abs( b ) - double.Abs( a ) );
        // Only of the same sign if these two differences are the same.
        // If not, d1 > d2
        return d1 <= d2 + 1E-8;

    }

    private int GetPivotRow( int col )
    {
        var last = Tableu.ColumnCount - 1;
        var jdx = 0;
        var target = double.MaxValue;
        for ( var i = 1; i < Tableu.RowCount; i++ )
        {
            double cand;
            if ( double.Abs( Tableu[i, col] ) < 1E-8 ||
                !SameSign( Tableu[i, col], Tableu[i, last] ) )
            {
                cand = double.MaxValue;
            }
            else
            {
                cand = Tableu[i, last] / Tableu[i, col];
            }
            if ( cand < target )
            {
                jdx = i;
                target = cand;
            }
        }
        return jdx;
    }

    public bool Iterate()
    {
        var pivotCol = GetPivotColumn();
        if ( pivotCol is null || currentIteration >= numIterations )
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
            if ( nonIntegersIndices.Count > 0 && !addedIntegerConstraints )
            {
                Console.WriteLine( $"Finished iterations. The following row indices are not integer {string.Join( ',', nonIntegersIndices )}" );
                var (A, b) = CreateIntegerConstraintsFor( nonIntegersIndices );
                // Console.WriteLine( $"New constraints are: {A} = {b}" );
                Console.WriteLine( Tableu );
                //AddInequalityConstraints( A, Vector<double>.Build.Dense( b.Count, 0.0 ) );
                AddInequalityConstraints( A, b );
                Console.WriteLine( $"{Tableu}" );
                addedIntegerConstraints = true;
                return true;
            }
            // We're done
            return true;
        }
        // Do iteration
        var pivotRow = GetPivotRow( pivotCol.Value );
        // Console.WriteLine( $"Pivoting about ({pivotRow}, {pivotCol.Value})" );
        Pivot( Tableu, pivotRow, pivotCol.Value );

        // Update basic vars:
        BasicVars[pivotRow - 1] = pivotCol.Value - 1;
        currentIteration++;
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
