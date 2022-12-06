namespace Day3;

public class Helpers
{
    public static (char[] lhs, char[] rhs) SplitLine(string line)
    {
        var len = line.Length;
        return (
            line.ToCharArray(0, len / 2),
            line.ToCharArray(len / 2, len / 2)
        );
    }
    public static int Score(char letter)
    {
        if (char.IsUpper(letter))
        {
            return letter - 'A' + 27;
        }
        else
        {
            return letter - 'a' + 1;
        }
    }

    public static char GetCommonLetter(IEnumerable<string> lines)
    {
        Console.WriteLine("GetCommonLetter");
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        return lines.Select(line => line.ToCharArray())
            .Aggregate((acc, item) => acc.Intersect(item).ToArray())
            .First();
    }
}
static class Extensions
{
    public static IEnumerable<string> GetLines(this StreamReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    public static IEnumerable<IEnumerable<string>> Clump(this IEnumerable<string> lines, int n)
    {
        Console.WriteLine($"Clump({n})");
        var asList = lines.ToList();
        int counter = 0;
        int N = asList.Count;
        Console.WriteLine(N);
        while (counter < N)
        {
            Console.WriteLine($"Yielding {n} lines. Counter is {counter}");
            yield return asList.Skip(counter).Take(n);
            counter += n;
        }
    }


}