namespace Day11;

public class Monkey
{
    public static ulong MaxValue { get; } = 9699690;
    public ulong Inspected { get; private set; } = 0;
    public Queue<ulong> Worries { get; init; } = new();
    public ulong Id { get; init; }
    public Func<ulong, ulong> Operation { get; set; } = default!;
    public Func<ulong, bool> Condition { get; set; } = default!;

    public Monkey IfTrue { get; set; } = default!;
    public Monkey IfFalse { get; set; } = default!;

    public void DoRound(bool getCalmed)
    {
        //Console.WriteLine($"Monkey {Id}");
        while (Worries.Count > 0)
        {
            Inspect(Worries.Dequeue(), getCalmed);
        }
    }

    public void Inspect(ulong worry, bool getCalmed)
    {
        // Update worry using operation:
        worry = Operation(worry) % MaxValue;

        // Monkeys is bored with the item:
        if (getCalmed) worry /= 3;

        Inspected++;
        // Check condition
        if (Condition(worry)) IfTrue.Worries.Enqueue(worry);
        else IfFalse.Worries.Enqueue(worry);
    }
}