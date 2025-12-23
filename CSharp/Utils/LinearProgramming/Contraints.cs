
using MathNet.Numerics.LinearAlgebra;

namespace AoC.Utils.LinearProgramming;

/// <summary>
/// Interface for representations of various types of linear contraints
/// </summary>
public interface ILinearConstraint { }

/// <summary>
/// Representation of a linear functional for use in linear programming
/// </summary>
/// <param name="Coefficients"></param>
public record LinearFunctional( Vector<double> Coefficients )
{
    public int Dimensions() => Coefficients.Count;

    public static LinearFunctional Constant( double value, int dim ) =>
        new LinearFunctional(
            Vector<double>.Build.Dense( dim, value )
        );
}

public record LinearOperator( Matrix<double> Coefficients )
{
    public static LinearOperator Identity( int n ) =>
        new LinearOperator( Matrix<double>.Build.DenseIdentity( n ) );
    public int DomainDimensions() => Coefficients.ColumnCount;
    public int RangeDimensions() => Coefficients.RowCount;

    public static ILinearConstraint operator <=( LinearOperator a, LinearFunctional b )
    {
        // First, we need to check that the dimensions agree:
        if ( a.RangeDimensions() != b.Dimensions() )
        {
            throw new ArgumentException( $"Dimension mismatch {a.RangeDimensions()} != {b.Dimensions()}" );
        }
        return new LessThanConstraint( a.Coefficients, b.Coefficients );
    }

    public static ILinearConstraint operator <=( LinearOperator a, double val ) =>
        a <= LinearFunctional.Constant( val, a.RangeDimensions() );


    public static ILinearConstraint operator >=( LinearOperator a, LinearFunctional b )
    {
        // First, we need to check that the dimensions agree:
        if ( a.RangeDimensions() != b.Dimensions() )
        {
            throw new ArgumentException( $"Dimension mismatch {a.RangeDimensions()} != {b.Dimensions()}" );
        }
        return new GreaterThanConstraint( a.Coefficients, b.Coefficients );
    }

    public static ILinearConstraint operator >=( LinearOperator a, double val ) =>
        a >= LinearFunctional.Constant( val, a.RangeDimensions() );

    public static ILinearConstraint operator ==( LinearOperator a, LinearFunctional b )
    {
        // First, we need to check that the dimensions agree:
        if ( a.RangeDimensions() != b.Dimensions() )
        {
            throw new ArgumentException( $"Dimension mismatch {a.RangeDimensions()} != {b.Dimensions()}" );
        }
        return new EqualityConstraint( a.Coefficients, b.Coefficients );
    }

    public static ILinearConstraint operator !=( LinearOperator a, LinearFunctional b )
    {
        throw new ArgumentException( $"'!=' makes no sense in this context." );
    }

    public static ILinearConstraint operator ^( LinearOperator a, LinearFunctional b )
    {
        // First, we need to check that the dimensions agree:
        if ( a.RangeDimensions() != b.Dimensions() )
        {
            throw new ArgumentException( $"Dimension mismatch {a.RangeDimensions()} != {b.Dimensions()}" );
        }
        return new BinaryConstraint( a.Coefficients, b.Coefficients );
    }

}
public record EqualityConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record LessThanConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record GreaterThanConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record BinaryConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record IntegerConstraint( int index ) : ILinearConstraint;

