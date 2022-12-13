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
            // Check overlapping children:
            foreach (var item in Children.Zip(tree.Children).ToList())
            {
                var sorted = item.First.IsSorted(item.Second);
                if (sorted == Sorted.False || sorted == Sorted.True)
                {
                    return sorted;
                }
            }
            // At this point all children were OK
            if (Children.Count() < tree.Children.Count()) return Sorted.True;
            else if (Children.Count() > tree.Children.Count()) return Sorted.False;
            else return Sorted.Continue;
        }
        else
        {
            var temp = new Tree();
            temp.Children.Add(node);
            return IsSorted(temp);
        }
    }
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
            var tempSize = ((Leaf)node).Size;
            if (Size < tempSize) return Sorted.True;
            else if (Size > tempSize) return Sorted.False;
            return Sorted.Continue;
        }
        else
        {
            var temp = new Tree { };
            temp.Children.Add(this);
            return temp.IsSorted(node);
        }
    }
}
