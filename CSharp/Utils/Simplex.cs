using MathNet.Numerics.LinearAlgebra;

namespace AoC.Utils;

public record SimplexProblem
{
    public required Matrix<double> Tableu { get; set; }
    public required int[] BasicVars { get; set; }
    public required int NumUnknowns { get; init; }
    public required int NumConstraints { get; init; }

    public Vector<double> CurrentSolution
    {
        get {
            var result = Vector<double>.Build.Dense( Tableu.ColumnCount - 2, 0.0 );
            for ( int i = 0; i < BasicVars.Count(); i++ )
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

        var tab = Matrix<double>.Build.Dense( 1 + n, 2 + m + n, 0.0 );
        // Set top row
        tab[0, 0] = 1.0;
        tab.SetSubMatrix( 0, 1, c.ToRowMatrix() );
        tab.SetSubMatrix( 1, 1, A );
        tab.SetSubMatrix( 1, m + 1, Matrix<double>.Build.DenseIdentity( n ) );
        tab.SetSubMatrix( 1, m + n + 1, b.ToColumnMatrix() );
        Console.WriteLine( $"{tab}" );
        // return
        return new SimplexProblem {
            Tableu = tab,
            BasicVars = Enumerable.Range( m, n ).ToArray(),
            NumUnknowns = m,
            NumConstraints = n
        };
    }

    private int? GetPivotColumn()
    {
        var objDiff = Tableu.SubMatrix( 0, 1, 1, Tableu.ColumnCount - 2 ).ToRowMajorArray();
        if ( objDiff.All( f => f < 1E-8 ) )
        {
            return null;
        }
        // Get index of largest element in objDiff
        var idx = 0;
        var target = double.MinValue;
        for ( var i = 0; i < objDiff.Length; i++ )
        {
            if ( objDiff[i] > target )
            {
                target = objDiff[i];
                idx = i;
            }
        }
        return 1 + idx;
    }

    private int GetPivotRow( int col )
    {
        var last = Tableu.ColumnCount - 1;
        var jdx = 0;
        var target = double.MaxValue;
        for ( var i = 1; i < Tableu.RowCount; i++ )
        {
            var cand = Tableu[i, last] / Tableu[i, col];
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
        Tableu = Pivot( Tableu, pivotRow, pivotCol.Value );

        // Update basic vars:
        var oldBasic = BasicVars[pivotRow - 1];
        var newBasic = pivotCol - 1;
        for ( int i = 0; i < BasicVars.Length; i++ )
        {
            if ( BasicVars[i] == oldBasic )
            {
                BasicVars[i] = newBasic.Value;
            }
        }
        // return
        return false;
    }

    private static Matrix<double> ScaleRow( Matrix<double> mat, int row, double scale )
    {
        mat.SetRow( row, mat.Row( row ) * scale );
        return mat;
    }

    private static Matrix<double> RowCombine( Matrix<double> mat, int row1, int row2, double factor )
    {
        mat.SetRow( row1, mat.Row( row1 ) + mat.Row( row2 ) * factor );
        return mat;
    }

    private static Matrix<double> Pivot( Matrix<double> mat, int row, int col )
    {
        if ( double.Abs( mat[row, col] ) < 1E-8 ) throw new Exception( "Cannot pivot around 0" );
        mat = ScaleRow( mat, row, 1.0 / mat[row, col] );
        // Combine every other row
        for ( int i = 0; i < mat.RowCount; i++ )
        {
            if ( i != row )
            {
                mat = RowCombine( mat, i, row, -mat[i, col] );
            }
        }
        return mat;
    }
}
