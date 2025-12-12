using System.Numerics;
namespace AoC.Utils;

public interface IGraphVisitedSet<T>
{
    bool Contains( T item );
    bool Add( T item );
}

record class GraphSet<T>( ISet<T> Set ) : IGraphVisitedSet<T>
{
    public bool Contains( T item ) => Set.Contains( item );
    public bool Add( T item ) => Set.Add( item );

    public static GraphSet<T> Empty() => new GraphSet<T>( new HashSet<T>() );
}

public static class Graph
{
    public static IEnumerable<(T node, long dist)> BFS<T>( T source, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T>
    {
        var queue = new Queue<(T node, long dist)>();
        queue.Enqueue( (source, 0L) );
        var visited = GraphSet<T>.Empty();
        return BFS( source, adjacentNodes, visited );
    }

    public static IEnumerable<(T node, long dist)> BFS<T>( T source, Func<T, IEnumerable<T>> adjacentNodes, IGraphVisitedSet<T> visited )
    where T : IEquatable<T>
    {
        var queue = new Queue<(T node, long dist)>();
        queue.Enqueue( (source, 0L) );
        while ( queue.TryDequeue( out var item ) )
        {
            var (node, dist) = item;
            if ( !visited.Contains( node ) )
            {
                visited.Add( node );
                yield return (node, dist);
                foreach ( var w in adjacentNodes( node ) )
                {
                    if ( !visited.Contains( w ) )
                    {
                        queue.Enqueue( (w, dist + 1) );
                    }
                }
            }
        }
    }

    public static IEnumerable<(TNode node, TWeight weight)> Dijkstra<TNode, TWeight>(
        TNode source,
        Func<TNode, IEnumerable<(TNode node, TWeight dist)>> adjacentNodes
    )
    where TNode : IEquatable<TNode>
    where TWeight : INumber<TWeight>
    {
        var seen = GraphSet<TNode>.Empty();
        return Dijkstra( source, adjacentNodes, seen );
    }

    public static IEnumerable<(TNode node, TWeight dist)> Dijkstra<TNode, TWeight>(
        TNode source,
        Func<TNode, IEnumerable<(TNode node, TWeight weight)>> adjacentNodes,
        IGraphVisitedSet<TNode> seen
    )
    where TNode : IEquatable<TNode>
    where TWeight : INumber<TWeight>
    {
        var queue = new PriorityQueue<TNode, TWeight>();
        queue.Enqueue( source, TWeight.Zero );
        while ( queue.TryDequeue( out var node, out var dist ) )
        {
            if ( !seen.Contains( node ) )
            {
                seen.Add( node );
                yield return (node, dist);
                foreach ( var (w, weight) in adjacentNodes( node ) )
                {
                    if ( !seen.Contains( w ) )
                    {
                        queue.Enqueue( w, dist + weight );
                    }
                }
            }
        }
    }

    public static IEnumerable<T> DFS<T>( T source, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T>
    {
        var explored = GraphSet<T>.Empty();
        return DFS( source, adjacentNodes, explored );
    }

    public static IEnumerable<T> DFS<T>(
        T source,
        Func<T, IEnumerable<T>> adjacentNodes,
        IGraphVisitedSet<T> explored )
    where T : IEquatable<T>
    {
        var stack = new Stack<(T node, IEnumerator<T> neighbours)>();
        // Set first element
        stack.Push( (source, adjacentNodes( source ).GetEnumerator()) );
        explored.Add( source );
        while ( stack.Count > 0 )
        {
            if ( stack.Peek().neighbours.MoveNext() )
            {
                var w = stack.Peek().neighbours.Current;
                if ( !explored.Contains( w ) )
                {
                    explored.Add( w );
                    stack.Push( (w, adjacentNodes( w ).GetEnumerator()) );
                }
            }
            else
            {
                yield return stack.Pop().node;
            }
        }
    }
}

public static partial class ShortestPath
{
    public static long BFS<T>( T source, T target, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T> => Graph.BFS( source, adjacentNodes ).First( item => item.node.Equals( target ) ).dist;
    public static long BFS<T>( T source, Func<T, bool> target, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T> => Graph.BFS( source, adjacentNodes ).First( item => target( item.node ) ).dist;


}

public class EmptySet<T> : IGraphVisitedSet<T>
{
    public bool Add( T item )
    {
        return false;
    }

    public void Clear()
    {
    }

    public bool Contains( T item )
    {
        return false;
    }
}
