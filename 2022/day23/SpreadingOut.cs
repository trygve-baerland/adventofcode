using Utils;
namespace Day23;

public static class SpreadingOut
{

    public static (List<Node<int>> updatedElves, bool movesMade) DoRound(List<Node<int>> elves)
    {
        var proposals = new List<(Node<int> position, int elf)>();
        // Part 1: Make proposals
        foreach (var (pos, elf) in elves.Select((pos, elf) => (pos, elf)))
        {
            // 1.1: Check if all 8 directions are clear:
            if (!GetSurroundingNodes(pos)
                .Where(node => elves.Contains(node))
                .Any()) continue;
            // 1.2: Consider each scan direction:
            foreach (var dir in ScanDirections)
            {
                if (!GetScanNodes(pos, dir)
                    .Where(node => elves.Contains(node))
                    .Any())
                {
                    // Add as proposed direction
                    proposals.Add((pos + dir, elf));
                    break;
                }
            }
        }
        bool movesMade = false;
        // Part 2: Enact proposals
        foreach (var (pos, elf) in proposals)
        {
            // Check that it's the only elf proposing this new position:
            if (proposals.Where(item => item.position == pos).Count() == 1)
            {
                movesMade = true;
                elves[elf] = pos;
            }
        }
        // Part 3: Update considered directions:
        var tmp = ScanDirections.First();
        ScanDirections.Remove(tmp);
        ScanDirections.Add(tmp);

        return (elves, movesMade);
    }

    public static List<(int x, int y)> ScanDirections { get; } = new() { (0, -1), (0, 1), (-1, 0), (1, 0) };

    public static IEnumerable<Node<int>> GetSurroundingNodes(Node<int> node) => AllDirections.Select(dir => node + dir);

    public static IEnumerable<Node<int>> GetScanNodes(Node<int> node, (int x, int y) dir)
    {
        if (dir.x != 0)
        {
            yield return node + dir + (0, -1);
            yield return node + dir;
            yield return node + dir + (0, 1);
        }
        else
        {
            yield return node + dir + (-1, 0);
            yield return node + dir;
            yield return node + dir + (1, 0);
        }
    }

    public static readonly IEnumerable<(int x, int y)> AllDirections = new List<(int x, int y)> {
        (1,0), (1,-1), (0,-1), (-1,-1), (-1, 0), (-1, 1), (0, 1), (1, 1)
    };

    public static int EvaluatePosition(List<Node<int>> elves)
    {
        var minX = elves.Select(node => node.X).Min();
        var maxX = elves.Select(node => node.X).Max();
        var minY = elves.Select(node => node.Y).Min();
        var maxY = elves.Select(node => node.Y).Max();
        return (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;
    }
}