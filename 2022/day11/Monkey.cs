namespace Day11;

public class Monkey
{
    public static ulong MaxValue { get; } = 9699690;
    public ulong Inspected { get; private set; } = 0;
    public Queue<ulong> Worries { get; init; } = new();
    public uint Id { get; init; }

    public uint Divisor { get; init; }
    public Func<ulong, ulong> Operation { get; init; } = default!;

    public uint IfTrue { get; init; }
    public uint IfFalse { get; init; }

    public IEnumerable<(uint, ulong)> DoRound(bool getCalmed)
    {
        while (Worries.Count > 0)
        {
            yield return Inspect(Worries.Dequeue(), getCalmed);
        }
    }

    private bool Condition(ulong worry)
    {
        return worry % Divisor == 0;
    }

    public (uint, ulong) Inspect(ulong worry, bool getCalmed)
    {
        // Update worry using operation:
        worry = Operation(worry) % MaxValue;

        // Monkeys is bored with the item:
        if (getCalmed) worry /= 3;

        Inspected++;
        // Check condition
        if (Condition(worry)) return (IfTrue, worry);
        else return (IfFalse, worry);
    }

}

public static class Extensions
{
    public static void DoRound(this Dictionary<uint, Monkey> monkeys, bool getCalmed)
    {
        foreach (var item in monkeys)
        {
            foreach (var (id, worry) in item.Value.DoRound(getCalmed))
            {
                monkeys[id].Worries.Enqueue(worry);
            }
        }
    }
}