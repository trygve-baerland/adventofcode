using System.Collections;
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
        return BFS( source, adjacentNodes, visited );
    }

    public static IEnumerable<(T node, long dist)> BFS<T>( T source, Func<T, IEnumerable<T>> adjacentNodes, ISet<T> visited )
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
        var explored = new HashSet<T>();
        return DFS( source, adjacentNodes, explored );
    }

    public static IEnumerable<T> DFS<T>(
        T source,
        Func<T, IEnumerable<T>> adjacentNodes,
        ISet<T> explored )
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

public class EmptySet<T> : ISet<T>
{
    public int Count => 0;

    public bool IsReadOnly => true;

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

    public void CopyTo( T[] array, int arrayIndex )
    {
        throw new NotImplementedException();
    }

    public void ExceptWith( IEnumerable<T> other )
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void IntersectWith( IEnumerable<T> other )
    {
        throw new NotImplementedException();
    }

    public bool IsProperSubsetOf( IEnumerable<T> other )
    {
        throw new NotImplementedException();
    }

    public bool IsProperSupersetOf( IEnumerable<T> other )
    {
        return true;
    }

    public bool IsSubsetOf( IEnumerable<T> other )
    {
        return true;
    }

    public bool IsSupersetOf( IEnumerable<T> other )
    {
        return false;
    }

    public bool Overlaps( IEnumerable<T> other )
    {
        return false;
    }

    public bool Remove( T item )
    {
        return false;
    }

    public bool SetEquals( IEnumerable<T> other )
    {
        return other.Count() == 0;
    }

    public void SymmetricExceptWith( IEnumerable<T> other )
    {
        throw new NotImplementedException();
    }

    public void UnionWith( IEnumerable<T> other )
    {
        throw new NotImplementedException();
    }

    void ICollection<T>.Add( T item )
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
