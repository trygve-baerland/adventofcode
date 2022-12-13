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

    public static IEnumerable<IEnumerable<T>> Clump<T>(this IEnumerable<T> lines, int n)
    {
        int N = lines.Count();
        int counter = 0;
        while (counter < N)
        {
            yield return lines.Skip(counter).Take(n);
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

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
}
