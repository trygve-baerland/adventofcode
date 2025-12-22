
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
public record LinearFunctional( Vector<double> Coefficients );

public record EqualityConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
public record LessThanConstraint( Matrix<double> A, Vector<double> B ) : ILinearConstraint;
