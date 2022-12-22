namespace Day21;

public enum Op
{
    Plus,
    Minus,
    Multiply,
    Divide,
    Compare,
}

public enum Direction
{
    L,
    R
}

public class ShoutingMonkey
{
    #region Instance attributes
    public string Name { get; private set; } = String.Empty;
    private decimal? _value;
    public decimal Value
    {
        get
        {
            if (_value is null)
            {
                if (Lhs is null || Rhs is null || Operation is null)
                {
                    throw new Exception("Insufficient data to make evaluation...");
                }
                var lhs = AllMonkeys[Lhs];
                var rhs = AllMonkeys[Rhs];
                _value = Operation switch
                {
                    Op.Plus => lhs.Value + rhs.Value,
                    Op.Minus => lhs.Value - rhs.Value,
                    Op.Multiply => lhs.Value * rhs.Value,
                    Op.Divide => lhs.Value / rhs.Value,
                    Op.Compare => Convert.ToDecimal(lhs.Value == rhs.Value),
                    _ => throw new Exception($"Unsupported operation {Operation}")
                };
            }
            return _value ?? throw new Exception("Something went wrong in evaluation");
        }
    }
    public string? Lhs { get; set; }
    public string? Rhs { get; set; }
    public Op? Operation { get; set; }
    #endregion Instance attributes

    #region Constructor
    public ShoutingMonkey(string name, decimal? value)
    {
        Name = name;
        _value = value;
        AllMonkeys.Add(Name, this);
    }
    #endregion Constructor
    public static Dictionary<string, ShoutingMonkey> AllMonkeys { get; } = new();

    #region Misc Helpers
    public IEnumerable<ShoutingMonkey> GetChildren()
    {
        if (Lhs is not null && Lhs != "")
            yield return AllMonkeys[Lhs];
        if (Rhs is not null && Rhs != "")
            yield return AllMonkeys[Rhs];
    }

    public ShoutingMonkey GetChild(Direction dir) => dir switch
    {
        Direction.L => AllMonkeys[Lhs!],
        Direction.R => AllMonkeys[Rhs!],
        _ => throw new Exception($"Unsupported direction '{dir}'")
    };

    public List<Direction> Find(string name)
    {
        var stack = new Stack<(ShoutingMonkey monkey, IEnumerator<ShoutingMonkey> children, List<Direction> trace)>();
        stack.Push((this, GetChildren().GetEnumerator(), new List<Direction>()));

        while (stack.Count > 0)
        {
            var (monkey, children, trace) = stack.Peek();
            if (children.MoveNext())
            {
                var next = children.Current;
                Direction dir;
                if (next.Name == monkey.Lhs) dir = Direction.L;
                else if (next.Name == monkey.Rhs) dir = Direction.R;
                else throw new Exception($"Unable to determine side of expression!");
                var nextTrace = new List<Direction>(trace)
                {
                    dir
                };
                stack.Push((next, next.GetChildren().GetEnumerator(), nextTrace));
            }
            else
            {
                // Visit monkey
                var item = stack.Pop();
                if (item.monkey.Name == name) return item.trace;
            }
        }
        throw new Exception($"Unable to find '{name}' from '{Name}'");
    }

    /// <summary>
    /// If we're at an expression (lhs op rhs) find the value that lhs or rhs needs to be so that the
    /// this node's value becomes target
    /// </summary>
    public decimal Invert(Direction dir, decimal target)
    {
        if (Lhs is null || Rhs is null || Operation is null) throw new Exception($"Cannot invert a leaf node.");
        var oppVal = dir switch
        {
            Direction.L => AllMonkeys[Rhs].Value,
            Direction.R => AllMonkeys[Lhs].Value,
            _ => throw new Exception($"Unsupported direction '{dir}'")
        };

        return Operation switch
        {
            Op.Plus => target - oppVal,
            Op.Multiply => target / oppVal,
            Op.Minus => dir switch
            {
                Direction.L => target + oppVal,
                Direction.R => oppVal - target,
                _ => throw new Exception($"Unsupported direction '{dir}'")
            },
            Op.Divide => dir switch
            {
                Direction.L => target * oppVal,
                Direction.R => oppVal / target,
                _ => throw new Exception($"Unsupported direction '{dir}'")
            },
            Op.Compare => oppVal,
            _ => throw new Exception($"Cannot invert operation '{Operation}'")
        };
    }

    public override string ToString()
    {
        string result = $"Monkey {Name}({Value})";
        return result;
    }
    #endregion Misc Helpers
}