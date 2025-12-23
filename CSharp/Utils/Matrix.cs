using MathNet.Numerics.LinearAlgebra;

namespace AoC.Utils;

/// <summary>
/// Some useful matrix operations.
/// </summary>
public static class Matrix
{
    public static void ScaleRow( this Matrix<double> mat, int row, double scale )
    {
        mat.SetRow( row, mat.Row( row ) * scale );
    }
    public static void RowCombine( this Matrix<double> mat, int row1, int row2, double factor )
    {
        mat.SetRow( row1, mat.Row( row1 ) + mat.Row( row2 ) * factor );
    }
    public static void Pivot( this Matrix<double> mat, int row, int col )
    {
        if ( double.Abs( mat[row, col] ) < 1E-8 ) throw new Exception( "Cannot pivot around 0" );
        mat.ScaleRow( row, 1.0 / mat[row, col] );
        // Combine every other row
        for ( int i = 0; i < mat.RowCount; i++ )
        {
            if ( i != row )
            {
                mat.RowCombine( i, row, -mat[i, col] );
            }
        }
    }
}
