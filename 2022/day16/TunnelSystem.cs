namespace Day16;

public class Valve
{
    public string Name { get; set; } = string.Empty;
    public int Rate { get; set; }

    public List<string> ConnectedValves { get; } = new List<string>();

    public string ToString()
    {
        string result = $"('{Name}', ";
        result += $"{Rate}, [";
        result += string.Join(',', ConnectedValves);
        result += "])";
        return result;
    }
}