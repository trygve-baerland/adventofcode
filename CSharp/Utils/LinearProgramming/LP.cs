
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

public class LP<T>
where T : ILinearProgram<T>
{
    private readonly T problem;

    private LP( T problem )
    {
        this.problem = problem;
    }
    public static LP<T> Minimize( LinearFunctional functional )
    {
        return new LP<T>( T.Minimize( functional ) );
    }

    // Add constrain to the program
    public LP<T> Given( ILinearConstraint constraint )
    {
        problem.AddConstraint( constraint );
        return this;
    }

    // Utility method for setting all unknowns as integers
    public LP<T> AllInteger()
    {
        foreach ( var index in Enumerable.Range( 0, problem.NumUnknowns ) )
        {
            problem.AddConstraint( new IntegerConstraint( index ) );
        }
        return this;
    }

    public double Solve()
    {
        while ( !problem.Iterate() ) { }
        return problem.CurrentObjective();
    }
}
