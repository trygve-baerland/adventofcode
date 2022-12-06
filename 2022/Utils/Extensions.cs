namespace Utils;
public static class Extensions
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
        var asList = lines.ToList();
        int counter = 0;
        int N = asList.Count;
        Console.WriteLine(N);
        while (counter < N)
        {
            yield return asList.Skip(counter).Take(n);
            counter += n;
        }
    }

    public static IEnumerable<char> GetChars(this StreamReader reader)
    {
        while (reader.Peek() >= 0)
        {
            yield return (char)reader.Read();
        }
    }
}
