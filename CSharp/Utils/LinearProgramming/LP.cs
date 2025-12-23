
using MathNet.Numerics.LinearAlgebra;

namespace AoC.Utils.LinearProgramming;

public interface ILinearProgram<T>
where T : ILinearProgram<T>
{
    int NumUnknowns { get; }
    int NumConstraints { get; }
    static abstract T Minimize( LinearFunctional functional );
    void AddConstraint( ILinearConstraint constraint );
    double CurrentObjective();
    Vector<double> CurrentSolution();
    bool Iterate();
}

public delegate void RefAction<T>( ref T item );

public class LP<T>
where T : ILinearProgram<T>
{
    public T Problem { get; init; }

    private LP( T problem )
    {
        Problem = problem;
    }
    public static LP<T> Minimize( LinearFunctional functional )
    {
        return new LP<T>( T.Minimize( functional ) );
    }

    // Add constrain to the program
    public LP<T> Given( ILinearConstraint constraint )
    {
        Problem.AddConstraint( constraint );
        return this;
    }

    // Utility method for setting all unknowns as integers
    public LP<T> AllInteger()
    {
        foreach ( var index in Enumerable.Range( 0, Problem.NumUnknowns ) )
        {
            Problem.AddConstraint( new IntegerConstraint( index ) );
        }
        return this;
    }

    public double Solve()
    {
        while ( !Problem.Iterate() ) { }
        return Problem.CurrentObjective();
    }

    public Vector<double> CurrentSolution() => Problem.CurrentSolution();
    public double CurrentObjective() => Problem.CurrentObjective();
}
