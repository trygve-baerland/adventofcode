using AoC.Utils;
using MathNet.Numerics.LinearAlgebra;
namespace AoC.Y2023;

public sealed class Day24 : IPuzzle
{
    public IEnumerable<HailStone> TestHail() => "2023/inputdata/day24_test.txt".GetLines().Select( HailStone.FromString ).ToList();
    public IEnumerable<HailStone> ActualHail() => "2023/inputdata/day24.txt".GetLines().Select( HailStone.FromString ).ToList();

    public void Part1()
    {
        var stones = ActualHail();
        var region = new Interval<long>( 200000000000000L, 400000000000000L );
        var result = stones.Pairs()
            .Count( pair => pair.Item1.IntersectsWith( pair.Item2, region ) );

        Console.WriteLine( result );
    }

    public void Part2()
    {
        var stones = ActualHail().Take( 3 ).ToList();
        var s1 = stones[0];
        var s2 = stones[1];
        var s3 = stones[2];

        var matrixArray = new Matrix<double>[,] {
            {(s1.Velocity - s2.Velocity).CrossMatrix(), (s2.Position - s1.Position).CrossMatrix()},
            {(s1.Velocity - s3.Velocity).CrossMatrix(), (s3.Position - s1.Position).CrossMatrix()}
        };
        var matrix = Matrix<double>.Build.DenseOfMatrixArray( matrixArray );

        var b = (s2.Cross() - s1.Cross()).Concatenate( s3.Cross() - s1.Cross() );

        var x = matrix.Solve( b );
        Console.WriteLine( $"result: {x[0] + x[1] + x[2]}" );
    }
}

internal static class Day24Helpers
{
    public static (double a, double b) Invert(
        Tangent2D<double> a,
        Tangent2D<double> b,
        Tangent2D<double> c
    )
    {
        var cross = a.Cross( b );
        if ( System.Math.Abs( cross ) < 1e-4 )
        {
            throw new ArgumentException( "Vectors are parallel" );
        }
        var ra = (( double ) c.Cross( b )) / cross;
        var rb = (( double ) a.Cross( c )) / cross;
        return (ra, rb);
    }
}


public record struct HailStone( Node3D<long> Position, Tangent3D<long> Velocity )
{
    public override string ToString() => $"pos={Position}, vel={Velocity}";
    public static HailStone FromString( string s )
    {
        var parts = s.Split( '@' );
        var pos = Node3D<long>.FromString( parts[0].TrimEnd() );
        var vel = Tangent3D<long>.FromString( parts[1].TrimStart() );
        return new HailStone( pos, vel );
    }

    public bool IntersectsWith( HailStone other, Interval<long> region )
    {
        try
        {
            var (ra, rb) = Day24Helpers.Invert(
                Velocity.To2D().To<double>(),
                (-other.Velocity).To2D().To<double>(),
                (other.Position - Position).To2D().To<double>()
            );
            // Get the position:
            var collision = Position + Velocity * (( long ) ra);
            return ra >= 0 && rb >= 0 &&
                region.Contains( collision.X ) &&
                region.Contains( collision.Y );
        }
        catch ( ArgumentException )
        {
            return false;
        }
    }
    public Tangent3D<long> Cross() => Position.ToTangent().Cross( Velocity );
}
