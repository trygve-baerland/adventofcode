namespace Day3;


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
