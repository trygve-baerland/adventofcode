using AoC.Utils;

namespace AoC.Y2022;

public sealed class Day7 : IPuzzle
{
    private Tree TestTree() =>
        Tree.ParseFromLines( "2022/inputdata/day7_test.txt".GetLines().GetEnumerator() );

    private Tree ActualTree() =>
        Tree.ParseFromLines( "2022/inputdata/day7.txt".GetLines().GetEnumerator() );

    public void Part1()
    {
        var tree = ActualTree();
        var smallDirs = new List<long>();
        tree.Visit( node => {
            if ( node.Size <= 100000 )
            {
                smallDirs.Add( node.Size );
            }
        } );
        var result = smallDirs.Sum();
        Console.WriteLine( result );
    }

    public void Part2()
    {
        var tree = ActualTree();
        int unused = 70000000 - tree.Size;
        int toBeFreed = 30000000 - unused;
        var canBeFreed = new List<int>();
        tree.Visit( node => {
            if ( node.Size >= toBeFreed )
            {
                canBeFreed.Add( node.Size );
            }
        } );

        Console.WriteLine( canBeFreed.Min() );

    }
}

public interface INode
{
    public string Name { get; set; }

    public INode? Parent { get; set; }

    public void ProcessLines( IEnumerator<string> lines );

    public int Size { get; }
}
public class Tree : INode
{
    public string Name { get; set; } = string.Empty;
    public INode? Parent { get; set; }

    public List<INode> Children { get; } = new();
    private int? _size = null;
    public int Size
    {
        get {
            _size ??= Children.Select( child => child.Size ).Sum();
            return _size ?? throw new Exception( "WHAT" );
        }
    }

    public INode GetChild( string name )
    {
        if ( !Children.Select( child => child.Name ).Contains( name ) )
        {
            throw new Exception( $"'{name}' is not a child of '{Name}'" );
        }
        return Children.Where( child => child.Name == name ).First();
    }

    public static Tree ParseFromLines( IEnumerator<string> lines )
    {
        // Create root:
        var tree = new Tree {
            Name = "root",
            Parent = null,
        };
        tree.Children.Add(
            new Tree {
                Name = "/",
                Parent = tree
            }
        );
        lines.MoveNext();
        tree.ProcessLines( lines );
        return tree;
    }

    public void ProcessLines( IEnumerator<string> lines )
    {
        var line = lines.Current;
        if ( line == null || !line.StartsWith( "$" ) )
        {
            throw new Exception( $"line '{line}' is not a command." );
        }
        // Get command and act accordingly:
        line = line.Trim( '$' ).Trim();
        var command = line.Split( " " )[0];
        switch ( command )
        {
            case "cd":
                ProcessCd( lines );
                break;
            case "ls":
                ProcessLs( lines );
                break;
            default:
                throw new Exception( $"Unknown command '{command}'" );
        }
    }

    public void ProcessCd( IEnumerator<string> lines )
    {
        var line = lines.Current;
        if ( line == null || !line.StartsWith( "$ cd" ) ) throw new Exception( $"line '{line}' is an invalid cd command" );
        var dirName = line.Split( " " )[2];
        if ( dirName == ".." )
        {
            if ( Parent is null )
            {
                throw new Exception( "Cannot move up without a parent" );
            }
            lines.MoveNext();
            Parent.ProcessLines( lines );
        }
        else
        {
            lines.MoveNext();
            GetChild( dirName ).ProcessLines( lines );
        }
    }

    public void ProcessLs( IEnumerator<string> lines )
    {
        while ( true )
        {
            if ( !lines.MoveNext() )
            {
                return;
            }
            var line = lines.Current;
            if ( line == null || line.StartsWith( "$" ) ) break;
            var items = line.Split( " " );
            if ( items[0] == "dir" )
            {
                Children.Add( new Tree {
                    Name = items[1],
                    Parent = this
                } );
            }
            else
            {
                var size = int.Parse( items[0] );
                Children.Add( new Leaf {
                    Name = items[1],
                    Parent = this,
                    Size = size
                } );
            }
        }
        ProcessLines( lines );
    }

    // Perform actions on a tree and all its children:
    public void Visit( Action<Tree> action )
    {
        action( this );
        foreach ( var child in Children )
        {
            if ( child.GetType() == typeof( Tree ) )
            {
                (( Tree ) child).Visit( action );
            }
        }
    }

}

public class Leaf : INode
{
    public string Name { get; set; } = string.Empty;
    public INode? Parent { get; set; }
    public int Size { get; set; } = 0;

    public void ProcessLines( IEnumerator<string> lines )
    { }
}
