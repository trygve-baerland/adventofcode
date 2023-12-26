using System.Collections;
using System.Numerics;
namespace AoC.Utils;

public class Index<T>( IEnumerable<T> items ) : IEnumerable<(T item, int index)>
where T : notnull
{
    private readonly Dictionary<T, int> _index =
        items.Select( ( item, i ) => (item, i) )
        .ToDictionary(
            item => item.item,
            item => item.i
        );
    public int this[T item] => _index[item];

    public int Count => _index.Count;

    public T Find( int index ) => _index.Where( item => item.Value == index )
        .Select( item => item.Key )
        .First();

    public IEnumerator<(T item, int index)> GetEnumerator() =>
        _index
        .Select( kvp => (kvp.Key, kvp.Value) )
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record class FloydWarshallMapping<T, TDist>( Index<T> Index, TDist[,] Distances )
where T : notnull
where TDist : INumber<TDist>, IMinMaxValue<TDist>
{
    public TDist this[T from, T to] => Distances[Index[from], Index[to]];
}

public static partial class ShortestPath
{
    public static FloydWarshallMapping<T, int> FloydWarshall<T>(
        IEnumerable<T> nodes,
        Func<T, IEnumerable<T>> adjacentNodes
    )
    where T : notnull =>
        FloydWarshall(
            nodes,
            node => adjacentNodes( node ).Select( w => (w, 1) )
        );

    public static FloydWarshallMapping<T, TDist> FloydWarshall<T, TDist>(
        IEnumerable<T> nodes,
        Func<T, IEnumerable<(T, TDist)>> adjacentNodes
    )
    where T : notnull
    where TDist : INumber<TDist>, IMinMaxValue<TDist>
    {
        nodes = nodes.ToList();
        var index = new Index<T>( nodes );
        // Initialize distance array
        var distances = new TDist[index.Count, index.Count];
        foreach ( var i in Enumerable.Range( 0, index.Count ) )
        {
            foreach ( var j in Enumerable.Range( 0, index.Count ) )
            {
                distances[i, j] = TDist.MaxValue;
            }
            distances[i, i] = TDist.Zero;
        }
        // Add edges:
        foreach ( var (node, i) in index )
        {
            foreach ( var (neighbour, dist) in adjacentNodes( node ) )
            {
                distances[i, index[neighbour]] = dist;
            }
        }

        // Do tropical matrix multiplication N times:
        foreach ( var k in Enumerable.Range( 0, index.Count ) )
        {
            foreach ( var i in Enumerable.Range( 0, index.Count ) )
            {
                foreach ( var j in Enumerable.Range( 0, index.Count ) )
                {
                    var candidate = Math.CheckedSum( distances[i, k], distances[k, j] );
                    if ( candidate < distances[i, j] )
                    {
                        distances[i, j] = candidate;
                    }
                }
            }
        }
        return new FloydWarshallMapping<T, TDist>( index, distances );
    }
}

