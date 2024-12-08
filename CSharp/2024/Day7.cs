using AoC.Utils;
using Microsoft.VisualBasic;
using Sprache;
namespace AoC.Y2024;

public sealed class Day7 : IPuzzle
{
    private readonly List<CalibrationEquation> TestEquations =
        "2024/inputdata/day7_test.txt".GetLines()
            .Select( Helpers.ACalibrationEquation.Parse )
            .ToList();

    private readonly List<CalibrationEquation> ProdEquations =
        "2024/inputdata/day7.txt".GetLines()
            .Select( Helpers.ACalibrationEquation.Parse )
            .ToList();

    public void Part1()
    {
        var eqs = ProdEquations;
        var result = eqs.Where( eq => eq.Check( false ) )
            .Select( eq => eq.TestValue )
            .Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var eqs = ProdEquations;
        var result = eqs.Where( eq => eq.Check( true ) )
            .Select( eq => eq.TestValue )
            .Sum();
        Console.WriteLine( result );
    }
}

internal record struct CalibrationEquation
{
    public long TestValue { get; init; }
    public List<long> Operands { get; init; }

    public override string ToString()
    {
        return $"{TestValue}: {string.Join( ' ', Operands )}";
    }

    public bool Check( bool withConcat )
    {
        var ops = Operands;
        var val = TestValue;
        return Graph.DFS(
            new EvaluationState {
                AtIndex = 1,
                Value = Operands[0]
            },
            state => state.NextFrom( ops, withConcat )
        ).Any( s => s.AtIndex == ops.Count && s.Value == val );
    }
}

internal record struct EvaluationState
{
    public int AtIndex { get; init; }
    public long Value { get; init; }

    public IEnumerable<EvaluationState> NextFrom( List<long> ops, bool withConcat )
    {
        if ( AtIndex < ops.Count )
        {
            var newVal = ops[AtIndex];
            yield return new EvaluationState {
                AtIndex = AtIndex + 1,
                Value = Value + newVal
            };
            yield return new EvaluationState {
                AtIndex = AtIndex + 1,
                Value = Value * newVal
            };
            if ( withConcat )
            {
                yield return new EvaluationState {
                    AtIndex = AtIndex + 1,
                    Value = Utils.Math.Concat( Value, newVal )
                };
            }
        }
    }
}

internal static partial class Helpers
{
    public readonly static Parser<long> ALong = Parse.Number.Select( s => long.Parse( s ) );
    public readonly static Parser<CalibrationEquation> ACalibrationEquation =
        ALong.Then( num => Parse.String( ": " ).Select( _ => num ) )
        .Then( num => ALong.DelimitedBy( Parse.WhiteSpace.AtLeastOnce() )
            .Select( ops => new CalibrationEquation {
                TestValue = num,
                Operands = ops.ToList()
            } ) );
}
