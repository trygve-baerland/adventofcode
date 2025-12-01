using AoC.Utils;
namespace AoC.Y2022;

public sealed class Day10 : IPuzzle
{
    private IEnumerable<string> TestOperations() =>
        "2022/inputdata/day10_test.txt".GetLines();
    private IEnumerable<string> ActualOperations() =>
        "2022/inputdata/day10.txt".GetLines();

    public void Part1()
    {
        VideoOperator videoOperator = new( 1, 0 );
        List<int> interestingCycles = new() { 20, 60, 100, 140, 180, 220 };
        int sum = 0;

        ActualOperations().ForEach(
            line => videoOperator.DoCommand( line, arg => {
                if ( interestingCycles.Contains( arg.Cycle ) )
                {
                    sum += arg.SignalStrength;
                }
            } )
        );

        // Part 1:
        Console.WriteLine( $"Part 1: {sum}" );
    }

    public void Part2()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string printOut = "";
        VideoOperator videoOperator = new( 1, 0 );
        ActualOperations()
        .ForEach(
            line => videoOperator.DoCommand( line, arg => {
                var pos = (arg.Cycle - 1) % 40;
                // Get position on line:
                if ( System.Math.Abs( arg.X - pos ) <= 1 )
                {
                    printOut += "\u258A";
                }
                else
                {
                    printOut += ".";
                }
                // Print out line if we're at end of line:
                if ( arg.Cycle % 40 == 0 )
                {
                    Console.WriteLine( printOut );
                    printOut = "";
                }
            } )
        );

    }
}
public enum Operation
{
    addx,
    noop,
}

public class VideoOperator
{
    public int X { get; set; } = 1;
    public int Cycle { get; set; } = 0;

    public int SignalStrength { get => X * Cycle; }

    public VideoOperator( int x, int cycle )
    {
        X = x;
        Cycle = cycle;
    }
    public void DoCommand( string line, Action<VideoOperator> action )
    {
        // Parse line:
        var items = line.Split( " " );
        if ( !Enum.TryParse( items[0], out Operation op ) )
        {
            throw new Exception( $"Unable to parse operation {items[0]}" );
        }
        else
        {
            var cycles = op switch {
                Operation.addx => DoAddx( int.Parse( items[1] ) ),
                Operation.noop => DoNoop(),
                _ => throw new Exception( $"Unknown command: {op}" )
            };
            foreach ( var _ in cycles )
            {
                action( this );
            }
        }

    }
    public IEnumerable<int> DoAddx( int num )
    {
        yield return NextCycle();
        yield return NextCycle();
        X += num;
    }

    public IEnumerable<int> DoNoop()
    {
        yield return NextCycle();
    }

    private int NextCycle()
    {
        Cycle++;
        return Cycle;
    }
}
