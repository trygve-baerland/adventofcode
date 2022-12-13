namespace Day13;

public enum Sorted
{
    True = -1,
    Continue = 0,
    False = 1,
}
public interface INode
{
    public INode? Parent { get; set; }
    public string ToString();
    public Sorted IsSorted(INode node);
    public INode ToTree();
}

public class Tree : INode
{
    public INode? Parent { get; set; }

    public List<INode> Children { get; } = new();

    public override string ToString()
    {
        string result = "[";
        result += string.Join(",", Children);
        result += "]";
        return result;
    }

    public Sorted IsSorted(INode node)
    {
        //Console.WriteLine($"Compare {this} with {node}");
        if (node.GetType() == typeof(Tree))
        {
            var tree = (Tree)node;
            return Children.Zip(tree.Children,
                (node1, node2) => node1.IsSorted(node2))
                .Where(sorted => sorted != Sorted.Continue)
                .FirstOrDefault(
                    (Children.Count() - tree.Children.Count()) switch
                    {
                        < 0 => Sorted.True,
                        > 0 => Sorted.False,
                        _ => Sorted.Continue
                    }
                );
        }
        else
        {
            return IsSorted(node.ToTree());
        }
    }

    public INode ToTree() => this;
}

public class Leaf : INode
{
    public INode? Parent { get; set; }
    public int Size { get; set; } = 0;

    public override string ToString()
    {
        return Size.ToString();
    }

    public Sorted IsSorted(INode node)
    {
        // Console.WriteLine($"Compare {this} with {node}");
        if (node.GetType() == typeof(Leaf))
        {
            return (Size - ((Leaf)node).Size) switch
            {
                < 0 => Sorted.True,
                > 0 => Sorted.False,
                _ => Sorted.Continue
            };
        }
        else
        {
            return ToTree().IsSorted(node);
        }
    }

    public INode ToTree()
    {
        var temp = new Tree { };
        temp.Children.Add(this);
        return temp;
    }
}
