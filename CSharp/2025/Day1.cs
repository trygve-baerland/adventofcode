using AoC.Utils;
using Sprache;

namespace AoC.Y2025;

public sealed class Day1 : IPuzzle
{
    public IEnumerable<(Rotation, long)> TestLines =
        "2025/inputdata/day1_test.txt".GetLines().Select(Helpers.RotationPair.Parse);
    public IEnumerable<(Rotation, long)> Lines =
        "2025/inputdata/day1.txt".GetLines().Select(Helpers.RotationPair.Parse).ToArray();

    public void Part1()
    {
        long result = 0;
        long pos = 50;
        foreach (var line in Lines )
        {
            pos = Helpers.DoRotation(pos, line);
            if (pos == 0) {
                result += 1;
            }
        }
        Console.WriteLine($"Result {result}");
    }

    public void Part2() { 

        long result = 0;
        long pos = 50;
        long clicks;
        foreach (var line in Lines )
        {
            (pos, clicks) = Helpers.DoRotationWithClicks(pos, line);
            result += clicks;
        }
        Console.WriteLine($"Result {result}");
    }
}

public enum Rotation
{
    Left,
    Right,
}

static partial class Helpers 
{
    private static readonly Parser<long> Long = Parse.Number.Select( long.Parse );
    private static readonly Parser<Rotation> RotationParser = 
        Parse.Char('L').Select(_ => Rotation.Left).Or(Parse.Char('R').Select(_ => Rotation.Right));
    public static readonly Parser<(Rotation, long)> RotationPair = 
        RotationParser.Then(r => Long.Select(num => (r, num)));

    public static long DoRotation(long num, (Rotation, long) rot) {
        return rot.Item1 switch {
            Rotation.Left => Utils.Math.MathMod(num - rot.Item2, 100),
            Rotation.Right => Utils.Math.MathMod(num + rot.Item2, 100),
            _ => throw new OverflowException("Unsupported rotation")
        };
    }

    public static (long, long) DoRotationWithClicks(long num, (Rotation, long) rot) {
        var newPos = rot.Item1 switch {
            Rotation.Left => num - rot.Item2,
            Rotation.Right => num + rot.Item2,
            _ => throw new OverflowException("Unsupported rotation")
        };
        var clicks = System.Math.Abs(newPos / 100);
        if (newPos <= 0 && num != 0) {
            clicks += 1;
        }
        
        return (Utils.Math.MathMod(newPos, 100), clicks); 

    }
}
