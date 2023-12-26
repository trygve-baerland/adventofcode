namespace AoC.Utils;

public static class Graph
{
    public static IEnumerable<(T node, long dist)> BFS<T>( T source, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T>
    {
        var queue = new Queue<(T node, long dist)>();
        queue.Enqueue( (source, 0L) );
        var visited = new HashSet<T>();
        while ( queue.Count > 0 )
        {
            var (node, dist) = queue.Dequeue();
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
}

public static class ShortestPath
{
    public static long BFS<T>( T source, T target, Func<T, IEnumerable<T>> adjacentNodes )
    where T : IEquatable<T> => Graph.BFS( source, adjacentNodes ).First( item => item.node.Equals( target ) ).dist;

}
