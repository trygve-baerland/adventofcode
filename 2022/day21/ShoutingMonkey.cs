namespace Day21;

public enum Op
{
    Plus,
    Minus,
    Multiply,
    Divide
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
}