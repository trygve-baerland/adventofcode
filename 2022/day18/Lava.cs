using Utils;

namespace Day18;

public class LavaClump
{
    public HashSet<Node3D<int>> Points = new();

    public int FreeSides { get; private set; } = 0;

    public void AddPoint(Node3D<int> point)
    {
        // First wee add 6 new sides:
        FreeSides += 6;
        // Then we subtract for every blocked side:
        foreach (var dir in Node3D<int>.Directions)
        {
            if (Points.Contains(point + dir))
            {
                FreeSides -= 2;
            }
        }
        // Finally, we add  the point to the clump:
        Points.Add(point);
    }
}