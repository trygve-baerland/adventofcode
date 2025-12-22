
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
}

public record LinearOperator( Matrix<double> Coefficients )
{
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
    public static ILinearConstraint operator >=( LinearOperator a, LinearFunctional b )
    {
        // First, we need to check that the dimensions agree:
        if ( a.RangeDimensions() != b.Dimensions() )
        {
            throw new ArgumentException( $"Dimension mismatch {a.RangeDimensions()} != {b.Dimensions()}" );
        }
        return new GreaterThanConstrain( a.Coefficients, b.Coefficients );
    }

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
}
public record EqualityConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record LessThanConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record GreaterThanConstrain( Matrix<double> A, Vector<double> B ) : ILinearConstraint;

public record struct IntegerConstraint( int index ) : ILinearConstraint;

