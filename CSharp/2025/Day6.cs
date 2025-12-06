
using AoC.Utils;
using AoC.Y2023;
using Sprache;

namespace AoC.Y2025;

public sealed class Day6 : IPuzzle
{
    private IEnumerable<CephalopodEquation> TestData =
        Helpers.HomeworkEquations.Parse( "2025/inputdata/day6_test.txt".GetText() ).ToList();
    private IEnumerable<CephalopodEquation> Data =
        Helpers.HomeworkEquations.Parse( "2025/inputdata/day6.txt".GetText() ).ToList();

    public void Part1()
    {
        var data = Data;
        var result = data.Select( eq => eq.Evaluate() ).Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var data = Data;
        foreach ( var eq in data )
        {
            Console.WriteLine( $"{string.Join( ',', eq.CephNumbers() )} = {eq.CephEvaluate()}" );
        }
        var result = data.Select( eq => eq.CephEvaluate() ).Sum();
        Console.WriteLine( result );
    }
}

static partial class Helpers
{
    public static readonly Parser<CephNumber> CephNumber =
        Long.Span().Select( span => new CephNumber( span.Value, span.Start.Column ) );


    public static readonly Parser<IEnumerable<CephNumber>> CephNumbers =
        Parse.Char( ' ' ).Many()
        .Then( _ =>
            CephNumber.DelimitedBy( Parse.Char( ' ' ).Many() )
        )
        .Then( nums => Parse.Char( ' ' ).Many().Select( _ => nums ) );

    public static readonly Parser<IEnumerable<char>> EquationOps =
        Parse.Char( '+' ).Or( Parse.Char( '*' ) )
        .DelimitedBy( Parse.WhiteSpace.AtLeastOnce() );

    public static readonly Parser<IEnumerable<CephalopodEquation>> HomeworkEquations =
        CephNumbers.DelimitedBy( Parse.LineEnd )
        .Then( numberLines => Parse.LineEnd.AtLeastOnce().Select( _ => numberLines ) )
        .Then( numberLines => EquationOps.Select( ops => {
            var iterators = numberLines.Select( l => l.GetEnumerator() ).ToList();
            var opsList = ops.ToList();
            var eqs = new List<List<CephNumber>>();
            while ( iterators.All( it => it.MoveNext() ) )
            {
                eqs.Add( iterators.Select( it => it.Current ).ToList() );
            }
            return eqs.Zip( ops )
                .Select( pair => new CephalopodEquation( pair.Item1, pair.Item2 ) );
        } ) );

}

record struct CephNumber( long number, int pos )
{
    public override readonly string ToString()
    {
        return $"({number}, {pos})";
    }

    public long RegularValue() => number;

    public int NumDigs() => ( int ) System.Math.Log10( number ) + 1;
    public long DigitAtPos( int x )
    {
        if ( x < pos )
        {
            return 0;
        }
        var digNum = x - pos;
        if ( digNum > NumDigs() )
        {
            return 0;
        }
        return (( int ) (number / System.Math.Pow( 10, NumDigs() - digNum - 1 ))) % 10;
    }
}

record struct CephalopodEquation( List<CephNumber> numbers, char op )
{
    public override readonly string ToString()
    {
        return $"{string.Join( ',', numbers )} '{op}'";
    }

    public long Evaluate() => op switch {
        '+' => numbers.Select( n => n.RegularValue() ).Sum(),
        '*' => numbers.Select( n => n.RegularValue() ).Aggregate( 1L, ( res, it ) => res * it ),
        _ => throw new ArgumentException( $"unsupported operand '{op}'" )
    };

    public long CephEvaluate() => op switch {
        '+' => CephNumbers().Sum(),
        '*' => CephNumbers().Aggregate( 1L, ( res, it ) => res * it ),
        _ => throw new ArgumentException( $"unsupported operand '{op}'" )
    };

    public IEnumerable<long> CephNumbers()
    {
        var start = numbers.Select( n => n.pos ).Min();
        var end = numbers.Select( n => n.NumDigs() ).Max();
        for ( int i = start + end - 1; i >= start; i-- )
        {
            long part = 0;
            foreach ( var number in numbers )
            {
                var dig = number.DigitAtPos( i );
                if ( dig == 0 && part != 0 )
                {
                    break;
                }
                part = 10 * part + number.DigitAtPos( i );
            }
            yield return part;
        }
    }

}
