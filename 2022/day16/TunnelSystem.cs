using Utils;
using Utils.Graphs;
namespace Day16;

public class Valve
{
    public string Name { get; set; } = string.Empty;
    public int Rate { get; set; }

    public List<string> ConnectedValves { get; } = new List<string>();

    public override string ToString()
    {
        string result = $"('{Name}', ";
        result += $"{Rate}, [";
        result += string.Join(',', ConnectedValves);
        result += "])";
        return result;
    }
}

public static class TunnelHelpers
{
    public static List<(Valve valve, int time)> OptimalRelease(
        Valve source,
        ShortestPathMapping<Valve> shortestPathMapping,
        int timeout
    )
    {
        // Initialize stuff:
        var stack = new Stack<(Valve node, IEnumerator<(Valve, int)> neighbours, int time, int total, List<(Valve valve, int time)> visited)>();
        stack.Push(
            (
                source,
                shortestPathMapping.GetFrom(source, item => -item.Rate).Where(item => item.node.Rate > 0).GetEnumerator(),
                0,
                0,
                new List<(Valve valve, int time)>() {
                    (source, 0)
                }
            )
        );
        var totals = new List<List<(Valve valve, int time)>>();
        while (stack.Count > 0)
        {
            var (v, neighbours, time, total, visited) = stack.Peek();
            var last = visited.Last();
            if (v.Name != last.valve.Name)
            {
                throw new Exception($"Data inconsistency in name {v.Name} != {last.valve.Name}");
            }
            if (time != last.time)
            {
                throw new Exception($"Data inconsistency in time {time} != {last.time}");
            }
            if (neighbours.MoveNext())
            {
                // We are considering going to w next:
                var (w, dist) = neighbours.Current;
                var newTime = time + dist + 1; // Going there and turning it on.
                if (newTime < timeout && !visited.Select(item => item.valve.Name).Contains(w.Name)) // We only care if can get there in time
                {
                    var newVisited = new List<(Valve valve, int time)>(visited) // I've been promised this makes a copy...
                    {
                        (w, newTime)
                    };
                    var newTotal = total + (timeout - newTime) * w.Rate;
                    stack.Push(
                        (
                            node: w,
                            neighbours: shortestPathMapping.GetFrom(w, item => -item.Rate).Where(item => item.node.Rate > 0).GetEnumerator(),
                            time: newTime,
                            total: newTotal,
                            visited: newVisited
                        )
                    );
                }
            }
            else
            {
                var item = stack.Pop();
                totals.Add(item.visited);
            }
        }
        // Get best path:
        Console.WriteLine($"Searched {totals.Count} paths");
        var pathVals = totals.Select((item, i) => EvaluatePath(item, timeout)).ToList();
        var index = pathVals.IndexOf(pathVals.Max());
        return totals[index];
    }

    public static int OptimalElephantRelease(
        Valve source,
        ShortestPathMapping<Valve> shortestPathMapping,
        int timeout
    )
    {
        // Initialize stuff:
        var stack = new Stack<(Valve node, IEnumerator<(Valve, int)> neighbours, int time, int total, List<(Valve valve, int time)> visited)>();
        stack.Push(
            (
                source,
                shortestPathMapping.GetFrom(source, item => -item.Rate).Where(item => item.node.Rate > 0).GetEnumerator(),
                0,
                0,
                new List<(Valve valve, int time)>() {
                    (source, 0)
                }
            )
        );
        var totals = new List<List<(Valve valve, int time)>>();
        while (stack.Count > 0)
        {
            var (v, neighbours, time, total, visited) = stack.Peek();
            var last = visited.Last();
            if (v.Name != last.valve.Name)
            {
                throw new Exception($"Data inconsistency in name {v.Name} != {last.valve.Name}");
            }
            if (time != last.time)
            {
                throw new Exception($"Data inconsistency in time {time} != {last.time}");
            }
            if (neighbours.MoveNext())
            {
                // We are considering going to w next:
                var (w, dist) = neighbours.Current;
                var newTime = time + dist + 1; // Going there and turning it on.
                if (newTime < timeout && !visited.Select(item => item.valve.Name).Contains(w.Name)) // We only care if can get there in time
                {
                    var newVisited = new List<(Valve valve, int time)>(visited) // I've been promised this makes a copy...
                    {
                        (w, newTime)
                    };
                    var newTotal = total + (timeout - newTime) * w.Rate;
                    stack.Push(
                        (
                            node: w,
                            neighbours: shortestPathMapping.GetFrom(w, item => -item.Rate).Where(item => item.node.Rate > 0).GetEnumerator(),
                            time: newTime,
                            total: newTotal,
                            visited: newVisited
                        )
                    );
                }
            }
            else
            {
                var item = stack.Pop();
                totals.Add(item.visited);
            }
        }
        // Get best path:
        Console.WriteLine($"Searched {totals.Count} paths");
        var pathVals = totals.Select((item, i) => EvaluatePath(item, timeout)).ToList();
        return totals.CrossProduct(totals)
            .Where(item => NoOverlap(item.Item1, item.Item2))
            .Select(item => EvaluatePath(item.Item1, timeout) + EvaluatePath(item.Item2, timeout))
            .Max();
    }

    public static int EvaluatePath(IEnumerable<(Valve valve, int time)> visited, int timeout)
    {
        return visited.Select(item => (timeout - item.time) * item.valve.Rate).Sum();
    }

    public static bool NoOverlap(IEnumerable<(Valve valve, int time)> path1, IEnumerable<(Valve valve, int time)> path2)
    {
        return !path1.Skip(1).Select(item => item.valve.Name)
            .Intersect(path2.Skip(1).Select(item => item.valve.Name))
            .Any();
    }
}