
namespace AoC.Utils.LinearProgramming;

public interface ILinearProgram<T>
where T : ILinearProgram<T>
{
    static abstract T Minimize( LinearFunctional functional );

    void AddConstraint( ILinearConstraint constraint );
    double CurrentObjective();
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

    public double Solve()
    {
        while ( !problem.Iterate() ) { }
        return problem.CurrentObjective();
    }
}
