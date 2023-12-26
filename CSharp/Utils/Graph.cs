using System.Numerics;
namespace AoC.Utils;

public static class Graph
{
    public static IEnumerable<(T node, long dist)> BFS<T>( T source, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T>
    {
        var queue = new Queue<(T node, long dist)>();
        queue.Enqueue( (source, 0L) );
        var visited = new HashSet<T>();
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

    public static IEnumerable<(T node, long dist)> Dijkstra<T>(
        T source,
        Func<T, IEnumerable<T>> adjacentNodes
    )
    where T : IEquatable<T>
    {
        var queue = new PriorityQueue<T, long>();
        queue.Enqueue( source, 0L );
        var seen = new HashSet<T>();
        while ( queue.TryDequeue( out var node, out var dist ) )
        {
            if ( !seen.Contains( node ) )
            {
                seen.Add( node );
                yield return (node, dist);
                foreach ( var w in adjacentNodes( node ) )
                {
                    if ( !seen.Contains( w ) )
                    {
                        queue.Enqueue( w, dist + 1 );
                    }
                }
            }
        }
    }

    public static IEnumerable<(TNode node, TWeight dist)> Dijkstra<TNode, TWeight>(
        TNode source,
        Func<TNode, IEnumerable<(TNode node, TWeight weight)>> adjacentNodes
    )
    where TNode : IEquatable<TNode>
    where TWeight : INumber<TWeight>
    {
        var queue = new PriorityQueue<TNode, TWeight>();
        queue.Enqueue( source, TWeight.Zero );
        var seen = new HashSet<TNode>();
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
        var stack = new Stack<(T node, IEnumerator<T> neighbours)>();
        var explored = new HashSet<T>();

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

}
