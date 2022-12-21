namespace Day19;

public class Cost
{
    public int Ore { get; set; } = 0;
    public int Clay { get; set; } = 0;
    public int Obsidian { get; set; } = 0;

    public override string ToString()
    {
        return $"({Ore} ore, {Clay} clay, {Obsidian} obsidian)";
    }
}

public class BluePrint
{
    public int Id { get; set; }
    public Cost OreRobot { get; set; } = default!;
    public Cost ClayRobot { get; set; } = default!;
    public Cost ObsidianRobot { get; set; } = default!;
    public Cost GeodeRobot { get; set; } = default!;

    public override string ToString()
    {
        string result = $"Blueprint {Id}:\n";
        result += $"\tOre robot: {OreRobot}\n";
        result += $"\tClay robot: {ClayRobot}\n";
        result += $"\tObsidian robot: {ObsidianRobot}\n";
        result += $"\tGeode robot: {GeodeRobot}\n";
        return result;
    }

    public IEnumerable<Cost> GetCosts()
    {
        yield return OreRobot;
        yield return ClayRobot;
        yield return ObsidianRobot;
        yield return GeodeRobot;
    }
    public int MaxOre { get => GetCosts().Select(cost => cost.Ore).Max(); }
}

public enum Construction
{
    OreRobot,
    ClayRobot,
    ObsidianRobot,
    GeodeRobot
}

public struct SearchState : IEquatable<SearchState>
{
    // Resources:
    public int Ore { get; set; } = 0;
    public int Clay { get; set; } = 0;
    public int Obsidian { get; set; } = 0;
    public int Geode { get; set; } = 0;
    // Robots:
    public int OreRobots { get; set; } = 0;
    public int ClayRobots { get; set; } = 0;
    public int ObsidianRobots { get; set; } = 0;
    public int GeodeRobots { get; set; } = 0;
    public Construction NextConstruction { get; set; }

    public SearchState() { }
    public void UpdateResources()
    {
        Ore += OreRobots;
        Clay += ClayRobots;
        Obsidian += ObsidianRobots;
        Geode += GeodeRobots;
    }

    public bool CanAfford(Cost cost)
    {
        return Ore >= cost.Ore && Clay >= cost.Clay && Obsidian >= cost.Obsidian;
    }

    public void EatCost(Cost cost)
    {
        Ore -= cost.Ore;
        Clay -= cost.Clay;
        Obsidian -= cost.Obsidian;
    }

    public SearchState Copy()
    {
        SearchState other = (SearchState)this.MemberwiseClone();

        return other;
    }

    public bool Equals(SearchState other)
    {
        return
            Ore == other.Ore &&
            Clay == other.Clay &&
            Obsidian == other.Obsidian &&
            Geode == other.Obsidian &&
            OreRobots == other.OreRobots &&
            ClayRobots == other.ClayRobots &&
            ObsidianRobots == other.ObsidianRobots &&
            GeodeRobots == other.GeodeRobots;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Ore);
        hash.Add(Clay);
        hash.Add(Obsidian);
        hash.Add(Geode);
        hash.Add(OreRobots);
        hash.Add(ClayRobots);
        hash.Add(ObsidianRobots);
        hash.Add(GeodeRobots);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is SearchState state && Equals(state);
    }

    public static bool operator ==(SearchState left, SearchState right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SearchState left, SearchState right)
    {
        return !(left == right);
    }
}

public static class Scheduling
{
    public static int MostGeodes(BluePrint blueprint, int remainingTurns, SearchState state)
    {
        bool robotConstructed = false;
        while (!robotConstructed && remainingTurns > 0)
        {
            switch (state.NextConstruction)
            {
                case Construction.OreRobot:
                    if (state.CanAfford(blueprint.OreRobot))
                    {
                        state.EatCost(blueprint.OreRobot);
                        robotConstructed = true;
                    }
                    break;
                case Construction.ClayRobot:
                    if (state.CanAfford(blueprint.ClayRobot))
                    {
                        state.EatCost(blueprint.ClayRobot);
                        robotConstructed = true;
                    }
                    break;
                case Construction.ObsidianRobot:
                    if (state.CanAfford(blueprint.ObsidianRobot))
                    {
                        state.EatCost(blueprint.ObsidianRobot);
                        robotConstructed = true;
                    }
                    break;
                case Construction.GeodeRobot:
                    if (state.CanAfford(blueprint.GeodeRobot))
                    {
                        state.EatCost(blueprint.GeodeRobot);
                        robotConstructed = true;
                    }
                    break;
            }
            state.UpdateResources();
            remainingTurns -= 1;
            if (robotConstructed)
            {
                switch (state.NextConstruction)
                {
                    case Construction.OreRobot:
                        state.OreRobots += 1;
                        break;
                    case Construction.ClayRobot:
                        state.ClayRobots += 1;
                        break;
                    case Construction.ObsidianRobot:
                        state.ObsidianRobots += 1;
                        break;
                    case Construction.GeodeRobot:
                        state.GeodeRobots += 1;
                        break;
                }
            }
        }

        int maxGeodes = state.Geode;
        if (remainingTurns > 0)
        {
            foreach (var nextRobot in Robots)
            {
                if (nextRobot == Construction.ObsidianRobot && state.ClayRobots == 0) continue;
                if (nextRobot == Construction.GeodeRobot && state.ObsidianRobots == 0) continue;
                if (
                    (nextRobot == Construction.OreRobot && state.OreRobots >= blueprint.MaxOre) ||
                    (nextRobot == Construction.ClayRobot && state.ClayRobots >= blueprint.ObsidianRobot.Clay) ||
                    (nextRobot == Construction.ObsidianRobot && state.ObsidianRobots >= blueprint.GeodeRobot.Obsidian)
                ) continue;
                var newState = state.Copy();
                newState.NextConstruction = nextRobot;
                var numGeodes = MostGeodes(blueprint, remainingTurns, newState);
                maxGeodes = Math.Max(maxGeodes, numGeodes);
            }
        }
        return maxGeodes;
    }

    public static readonly List<Construction> Robots = new() {
        Construction.OreRobot,
        Construction.ClayRobot,
        Construction.ObsidianRobot,
        Construction.GeodeRobot
    };
}