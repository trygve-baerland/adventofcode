using Utils;
namespace Day20;

public class Element
{
    public long Value { get; set; }
    public long Position { get; set; }
    public long InitialPosition { get; init; }

    public void Update(int step, int N)
    {
        Position = Helpers.MathMod(Position + step, N);
    }
}

public class PeriodicInterval
{
    public long From { get; set; }
    public long To { get; set; }
    public long N { get; set; }
    private long D { get; init; }

    public PeriodicInterval(long from, long to, long N)
    {
        this.N = N;
        From = from;
        To = to;
        D = Helpers.MathMod(To - From, N);
    }

    public bool Contains(long value)
    {
        return Helpers.MathMod(value - From, N) <= D;
    }
}



public static class Decryption
{
    public static List<Element> MixDecrypt(List<Element> elements, int repetitions = 1)
    {
        //Console.WriteLine("Initial list:");
        //PrintList(elements);
        int N = elements.Count;
        foreach (var _ in Enumerable.Range(0, repetitions))
        {
            foreach (var element in elements)
            {
                //Console.WriteLine($"Updating using value {element.Value}");
                // Update Position of element
                var oldPos = element.Position;
                // If you move N-1 either to the left or the right, the array remains unchanged, save for a right- or left shift, respectively.
                // Taking this account, the new position of our element will be its value % (N-1)
                var offset = Math.Sign(element.Value) * (Math.Abs(element.Value) % (N - 1));
                element.Position = Helpers.MathMod(element.Position + offset, N);
                // everything between [min, max] is updated:
                var step = -Math.Sign(element.Value);
                PeriodicInterval interval;
                if (step < 0)
                {
                    interval = new PeriodicInterval(oldPos, element.Position, N);
                }
                else
                {
                    interval = new PeriodicInterval(element.Position, oldPos, N);
                }
                // Update positions for every element:
                foreach (var toUpdate in elements)
                {
                    if (interval.Contains(toUpdate.Position) && toUpdate.InitialPosition != element.InitialPosition)
                    {
                        toUpdate.Update(step, N);
                    }
                }
                //PrintList(elements);
            }
        }
        return elements;
    }

    public static void PrintList(List<Element> elements)
    {
        Console.WriteLine(
        string.Join(
            ", ",
        elements.OrderBy(item => item.Position)
                .Select(element => (element.Value, element.Position))
        )
        );
    }

    public static long GetValueInPosition(List<Element> elements, long pos)
    {
        int N = elements.Count;
        pos = Helpers.MathMod(pos, N);
        return elements.Where(item => item.Position == pos).Select(item => item.Value).First();
    }
}