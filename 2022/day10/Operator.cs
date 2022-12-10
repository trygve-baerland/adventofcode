namespace Day10;

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

    public VideoOperator(int x, int cycle)
    {
        X = x;
        Cycle = cycle;
    }
    public void DoCommand(string line, Action<VideoOperator> action)
    {
        // Parse line:
        var items = line.Split(" ");
        if (!Enum.TryParse(items[0], out Operation op))
        {
            throw new Exception($"Unable to parse operation {items[0]}");
        }
        else
        {
            var cycles = op switch
            {
                Operation.addx => DoAddx(int.Parse(items[1])),
                Operation.noop => DoNoop(),
                _ => throw new Exception($"Unknown command: {op}")
            };
            foreach (var _ in cycles)
            {
                action(this);
            }
        }

    }
    public IEnumerable<int> DoAddx(int num)
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