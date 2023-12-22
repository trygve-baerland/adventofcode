using Utils;

namespace AoC;

public sealed class Day22 : IPuzzle
{
    public List<Cube> TestCubes() => "inputdata/day22_test.txt"
        .GetLines()
        .Select( Cube.Parse )
        .ToList();

    public List<Cube> ActualCubes() => "inputdata/day22.txt"
        .GetLines()
        .Select( Cube.Parse )
        .ToList();

    public void Part1()
    {
        var cubes = ActualCubes();
        var stack = new SandStack();
        stack.LetFall( cubes );

        var result = stack.Cubes
            .Where( c => c.CanBeDisintegrated() )
            .Select( c => c.MaxHeight() )
            .Count();

        Console.WriteLine( $"Result: {result}" );
    }

    public void Part2()
    {
        var cubes = ActualCubes();
        var stack = new SandStack();
        stack.LetFall( cubes );
        var result = stack.Cubes
            .Select( f => f.Disintegrate() )
            .Sum();

        Console.WriteLine( $"Result: {result}" );
    }
}

public record struct Rectangle( Interval X, Interval Y )
{
    public override string ToString() => $"({X}, {Y})";

    public bool Overlaps( Rectangle other ) =>
        X.Intersects( other.X ) && Y.Intersects( other.Y );
}

public record struct Cube( Rectangle Horizontal, Interval Z )
{
    public override string ToString() => $"({Horizontal}, {Z})";

    public static Cube Parse( string input )
    {
        var parts = input.Split( '~' );
        var starts = parts[0].Split( ',' );
        var ends = parts[1].Split( ',' );
        return new Cube(
            new Rectangle(
                new Interval( long.Parse( starts[0] ), long.Parse( ends[0] ) ),
                new Interval( long.Parse( starts[1] ), long.Parse( ends[1] ) )
            ),
            new Interval( long.Parse( starts[2] ), long.Parse( ends[2] ) )
        );
    }

    public bool VerticallyOverlaps( Cube other ) =>
        Horizontal.Overlaps( other.Horizontal );
}

public record class FallingSandCube
{
    // All cubes that this supports
    private List<FallingSandCube> Supports { get; } = new();
    // All cubes that support this.
    private List<FallingSandCube> SupportedBy { get; } = new();

    public Cube Cube { get; init; }

    public long AtHeight { get; init; }
    public long Id { get; init; }

    public bool CanBeDisintegrated()
    {
        // If it doesn't support anything, we're in the clear
        if ( Supports.Count == 0 ) return true;
        // Otherwise, we need to check that every cube supported by it,
        // is supported by something else
        return Supports.All( s => s.SupportedBy.Count > 1 );
    }

    public long MaxHeight() => AtHeight + Cube.Z.Length();

    public void RestsOn( FallingSandCube other )
    {
        this.SupportedBy.Add( other );
        other.Supports.Add( this );
    }

    /// <summary>
    /// If we disintegrate this cube, how many cubes will fall?
    /// </summary>
    /// <returns></returns>
    public long Disintegrate( List<long>? fallen = null )
    {
        fallen ??= new List<long>();
        // We let this cube "fall":
        fallen.Add( Id );
        var result = 0L;
        // For each of the cube that we support, we check if it now starts falling
        foreach ( var supported in Supports )
        {
            if ( supported.SupportedBy
                .Select( f => f.Id )
                .All( id => fallen.Contains( id ) ) )
            {
                result += 1 + supported.Disintegrate( fallen );
            }
        }
        return result;
    }
}

public class SandStack
{
    public List<FallingSandCube> Cubes { get; } = new();

    public void LetFall( Cube cube )
    {
        // First, at what height this cube will fall:
        var atHeight = Cubes
            .Where( c => c.Cube.VerticallyOverlaps( cube ) )
            .Select( c => c.MaxHeight() )
            .DefaultIfEmpty( 0 )
            .Max();

        var falling = new FallingSandCube {
            Cube = cube,
            AtHeight = atHeight,
            Id = Cubes.Count
        };

        // Now we add all support connections that are needed:
        foreach ( var landed in Cubes )
        {
            if ( landed.MaxHeight() == falling.AtHeight && landed.Cube.VerticallyOverlaps( falling.Cube ) )
            {
                falling.RestsOn( landed );
            }
        }
        // Finally, we add the cube to the stack:
        Cubes.Add( falling );
    }

    public void LetFall( IEnumerable<Cube> cubes )
    {
        foreach ( var cube in cubes.OrderBy( c => c.Z.Start ) )
        {
            LetFall( cube );
        }
    }
}

