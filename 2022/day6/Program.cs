using Utils;

// Parse input
StreamReader reader = File.OpenText("input.txt");
char[] buffer = new char[14];
reader.Read(buffer, 0, 14);

if (Helpers.Distinct(buffer)) Console.WriteLine($"Result: {14}");
// Go through remaining characters:
int index = 0;
foreach (var c in reader.GetChars())
{
    buffer[index % 14] = c;
    if (Helpers.Distinct(buffer)) break;
    index++;
}
Console.WriteLine($"Result: {index + 15}");

class Helpers
{
    public static bool Distinct(char[] arr)
    {
        return arr.Distinct().Count() == arr.Count();
    }
}

