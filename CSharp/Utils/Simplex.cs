using MathNet.Numerics.LinearAlgebra;

namespace AoC.Utils;

public record SimplexProblem
{
    public required Matrix<double> Tableu { get; set; }
    public required int[] BasicVars { get; set; }
    public required int NumUnknowns { get; init; }
    public required int NumConstraints { get; init; }
    public int numIterations = 100;
    private int currentIteration = 0;
    private static double M = 10000;

    public long CurrentObjective => ( long ) -Tableu[0, Tableu.ColumnCount - 1];

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

        var tab = Matrix<double>.Build.Dense( 1 + n, m + n + 2, 0.0 );
        // Set up tableau matrix
        tab[0, 0] = 1.0;
        tab.SetSubMatrix( 0, 1, c.ToRowMatrix() );
        tab.SetSubMatrix( 1, 1, A );
        tab.SetSubMatrix( 1, m + 1, Matrix<double>.Build.DenseIdentity( n ) );
        tab.SetSubMatrix( 1, m + n + 1, b.ToColumnMatrix() );
        // Set up M-constraints:
        tab.SetSubMatrix( 0, m + 1, Vector<double>.Build.Dense( n, M ).ToRowMatrix() );
        // The current value for the objective functional will be updated after the Initialize call.
        //tab[0, tab.ColumnCount - 1] = M * b.Sum();

        return new SimplexProblem {
            Tableu = tab,
            // BasicVars is a collection of the column indices for the current BFS (Basic feasible solution)
            // Their corresponding coefficients are found, I think, in the last column.
            // We start with the BFS that we only use the newly introduced variables,
            // which can be found in indices m, m+1,...,m+n
            BasicVars = Enumerable.Range( m, n ).ToArray(),
            NumUnknowns = m,
            NumConstraints = n
        };
    }

    // Run through inverse iteration n times to get initial tableau
    public void Initialize()
    {
        for ( int i = 0; i < NumConstraints; i++ )
        {
            Pivot( Tableu, i + 1, NumUnknowns + i + 1 );
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
        // Console.WriteLine( $"Pivot column is {idx}" );
        return idx + 1;
    }

    private static bool SameSign( double a, double b )
    {
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
        if ( pivotCol is null )
        {
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
        // return
        return false;
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
