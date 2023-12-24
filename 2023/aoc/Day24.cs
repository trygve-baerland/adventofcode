using MathNet.Numerics.LinearAlgebra;
using Utils;
namespace AoC;

public sealed class Day24 : IPuzzle
{
    public IEnumerable<HailStone> TestHail() => "inputdata/day24_test.txt".GetLines().Select( HailStone.FromString ).ToList();
    public IEnumerable<HailStone> ActualHail() => "inputdata/day24.txt".GetLines().Select( HailStone.FromString ).ToList();

    public void Part1()
    {
        var stones = ActualHail();
        var region = new DoubleInterval( 200000000000000, 400000000000000 );
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

public record struct Point3D( long X, long Y, long Z )
{
    public static Point3D operator +( Point3D a, TangentVector3D b ) => new( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
    public static TangentVector3D operator -( Point3D a, Point3D b ) => new( a.X - b.X, a.Y - b.Y, a.Z - b.Z );
    public override string ToString() => $"({X},{Y},{Z})";

    public static Point3D FromString( string s )
    {
        var parts = s.Split( ',' );
        return new Point3D( long.Parse( parts[0].TrimEnd() ), long.Parse( parts[1].TrimEnd() ), long.Parse( parts[2].TrimEnd() ) );
    }

    public TangentVector3D ToVector() => new( X, Y, Z );
}

public record struct TangentVector3D( long X, long Y, long Z )
{
    public static TangentVector3D operator +( TangentVector3D a, TangentVector3D b ) => new( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
    public static TangentVector3D operator -( TangentVector3D a, TangentVector3D b ) => new( a.X - b.X, a.Y - b.Y, a.Z - b.Z );
    public static TangentVector3D operator -( TangentVector3D a ) => new( -a.X, -a.Y, -a.Z );
    public static TangentVector3D operator *( TangentVector3D a, long b ) => new( a.X * b, a.Y * b, a.Z * b );
    public static TangentVector3D operator *( double b, TangentVector3D a ) => b * a;

    public TangentVector3D CrossProduct( TangentVector3D other ) =>
        new(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );
    public override string ToString() => $"[{X},{Y},{Z}]";

    public static TangentVector3D FromString( string s )
    {
        var parts = s.Split( ',' );
        return new TangentVector3D( long.Parse( parts[0].TrimEnd() ), long.Parse( parts[1].TrimEnd() ), long.Parse( parts[2].TrimEnd() ) );
    }

    public TangentVector2D To2D() => new( X, Y );

    public Matrix<double> CrossMatrix()
    {
        var builder = Matrix<double>.Build;
        return builder.DenseOfArray( new double[,] {
            {0, -Z, Y},
            {Z, 0, -X},
            {-Y, X, 0}
        } );
    }

    public MathNet.Numerics.LinearAlgebra.Vector<double> Concatenate( TangentVector3D other ) =>
        MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense( new double[] { X, Y, Z, other.X, other.Y, other.Z } );
}

public record struct TangentVector2D( double X, double Y )
{
    public override string ToString() => $"[{X},{Y}]";

    public double CrossProduct( TangentVector2D other ) => X * other.Y - Y * other.X;

    public static (double a, double b) Invert( TangentVector2D a, TangentVector2D b, TangentVector2D c )
    {
        var cross = a.CrossProduct( b );
        if ( System.Math.Abs( cross ) < 1e-4 )
        {
            throw new ArgumentException( "Vectors are parallel" );
        }
        var ra = (( double ) c.CrossProduct( b )) / cross;
        var rb = (( double ) a.CrossProduct( c )) / cross;
        return (ra, rb);
    }
}

public record struct DoubleInterval( double A, double B )
{
    public bool Contains( double x ) => x >= A && x <= B;
}

public record struct HailStone( Point3D Position, TangentVector3D Velocity )
{
    public override string ToString() => $"pos={Position}, vel={Velocity}";
    public static HailStone FromString( string s )
    {
        var parts = s.Split( '@' );
        var pos = Point3D.FromString( parts[0].TrimEnd() );
        var vel = TangentVector3D.FromString( parts[1].TrimStart() );
        return new HailStone( pos, vel );
    }

    public bool IntersectsWith( HailStone other, DoubleInterval region )
    {
        try
        {
            var (ra, rb) = TangentVector2D.Invert(
                Velocity.To2D(),
                (-other.Velocity).To2D(),
                (other.Position - Position).To2D()
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
    public TangentVector3D Cross() => Position.ToVector().CrossProduct( Velocity );
}
