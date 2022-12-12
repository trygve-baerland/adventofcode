namespace Day12;

public static class BFS
{
    public static (int row, int col) IndexOf(char[][] arr, char letter)
    {
        var (line, row) = arr.Select((line, ind) => (line, ind))
            .Where(item => item.line.Contains(letter))
            .First();

        var col = Array.IndexOf(line, letter);
        return (row, col);

    }

    public static int ShortestPath(char[][] arr, (int row, int col) source, (int row, int col) target)
    {
        // Initialize stuff:
        int NRow = arr.Length;
        var toVisit = new Queue<((int row, int col) index, int dist)>();
        toVisit.Enqueue((source, 0));
        var explored = new HashSet<(int row, int col)>();

        // Main loop:
        while (toVisit.Count > 0)
        {
            var (v, dist) = toVisit.Dequeue();
            if (v == target) return dist;
            if (!explored.Contains(v))
            {
                explored.Add(v);
                foreach (var w in AdjacentNodes(arr, v))
                {
                    toVisit.Enqueue((w, dist + 1));
                }
            }
        }
        return int.MaxValue;
    }

    public static readonly IEnumerable<(int row, int col)> Directions = new List<(int row, int col)> { (-1, 0), (1, 0), (0, -1), (0, 1) };

    public static (int row, int col) Add((int row, int col) s, (int row, int col) t)
    {
        return (s.row + t.row, s.col + t.col);
    }
    public static IEnumerable<(int row, int col)> AdjacentNodes(char[][] arr, (int row, int col) index)
    {
        char s = arr[index.row][index.col];
        int NRow = arr.Length;
        int NCol = arr[index.row].Length;
        foreach (var dir in Directions)
        {
            var targetIndex = Add(index, dir);
            if (targetIndex.row > 0 && targetIndex.row < NRow &&
                targetIndex.col > 0 && targetIndex.col < NCol &&
                CanTraverse(arr, index, targetIndex))
            {
                yield return targetIndex;
            }
        }
    }

    public static bool CanTraverse(char[][] arr, (int row, int col) s, (int row, int col) t)
    {
        return EdgeWeight(arr, s, t) < 2;
    }

    public static int EdgeWeight(char[][] arr, (int row, int col) source, (int row, int col) target)
    {
        char s = MapChar(arr[source.row][source.col]);
        char t = MapChar(arr[target.row][target.col]);
        if (t - s > 1)
        {
            return int.MaxValue;
        }
        else
        {
            return 1;
        }
    }

    public static char MapChar(char c)
    {
        if (c == 'S') return 'a';
        if (c == 'E') return 'z';
        return c;
    }

    public static int GetCandidateDistance(int oldDistance, int edgeWeight)
    {
        try
        {
            return checked(oldDistance + edgeWeight);
        }
        catch (OverflowException)
        {
            return int.MaxValue;
        }
    }


    /* Code for part 2: */
    public static Dictionary<((int row, int col), (int row, int col)), int> FloydWarshall(char[][] arr)
    {
        // Initialize stuff:
        Dictionary<((int row, int col), (int row, int col)), int> dist = new();

        foreach (var idx in GetIndices(arr))
        {
            foreach (var idy in GetIndices(arr))
            {
                dist.Add((idx, idy), int.MaxValue);
            }
            dist[(idx, idx)] = 0;
            foreach (var idy in AdjacentNodes(arr, idx))
            {
                dist[(idx, idy)] = 1;
            }
        }
        // Make iterations:
        foreach (var idk in GetIndices(arr))
        {
            foreach (var idi in GetIndices(arr))
            {
                foreach (var idj in GetIndices(arr))
                {
                    var candidate = GetCandidateDistance(dist[(idi, idk)], dist[(idk, idj)]);
                    if (dist[(idi, idj)] > candidate)
                    {
                        dist[(idi, idj)] = candidate;
                    }
                }
            }
        }
        return dist;
    }

    public static IEnumerable<(int row, int col)> GetIndices(char[][] arr)
    {
        foreach (var ix in Enumerable.Range(0, arr.Length))
        {
            foreach (var iy in Enumerable.Range(0, arr[ix].Length))
            {
                yield return (ix, iy);
            }
        }
    }

}