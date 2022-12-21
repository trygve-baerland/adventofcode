using Utils;
namespace Day20;

public class Element
{
    public int Value { get; set; }
    public int Position { get; set; }
    public int InitialPosition { get; init; }

    public void Update(int step)
    {
        Position += step;
    }
}

public static class Decryption
{
    public static List<Element> MixDecrypt(List<Element> elements)
    {
        Console.WriteLine("Initial list:");
        PrintList(elements);
        int N = elements.Count;
        foreach (var element in elements)
        {
            Console.WriteLine($"Updating using value {element.Value}");
            // Update Position of element
            var oldPos = element.Position;
            element.Position = Helpers.MathMod(element.Position + element.Value, N);
            // everything between [min, max] is updated:
            var step = Math.Sign(oldPos - element.Position);
            var min = Math.Min(oldPos, element.Position);
            var max = Math.Max(oldPos, element.Position);
            // Update positions for every element:
            foreach (var toUpdate in elements)
            {
                if (toUpdate.Position >= min && toUpdate.Position <= max && toUpdate.InitialPosition != element.InitialPosition)
                {
                    toUpdate.Update(step);
                }
            }
            PrintList(elements);
        }
        return elements;
    }

    public static void PrintList(List<Element> elements)
    {
        Console.WriteLine(
        string.Join(
            ", ",
        elements.OrderBy(item => item.Position)
                .Select(element => element.Value)
        )
        );
    }
}