using System.Text;
using AoC.Y2023;

namespace AoC.Utils;

internal record class Map<T>( T[][] Data )
where T : struct, IEquatable<T>
{
    public int Height => Data.Length;
    public int Width => Data[0].Length;
    public T this[Node2D<int> node] => Data[node.X][node.Y];

    public virtual bool Contains( Node2D<int> node ) =>
        node.X >= 0 && node.X < Height && node.Y >= 0 && node.Y < Width;
    public IEnumerable<Node2D<int>> Coordinates() =>
        Enumerable.Range( 0, Height )
        .SelectMany( i =>
            Enumerable.Range( 0, Width )
            .Select( j => new Node2D<int>( i, j ) )
        );

    public IEnumerable<T> Row( int row ) => Data[row];
    public IEnumerable<T> Column( int column ) => Data.Select( r => r[column] );

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach ( var row in Data )
        {
            builder.AppendLine( string.Concat( row ) );
        }
        return builder.ToString();
    }

    public Node2D<int> Find( T item ) =>
        Coordinates().First( node => this[node].Equals( item ) );
}

internal record class CharMap( char[][] Data ) : Map<char>( Data )
{

    public static CharMap FromFile( string path ) =>
        new( path.GetLines().Select( line => line.ToCharArray() ).ToArray() );

}

internal record class IntMap( int[][] Data ) : Map<int>( Data )
{

}
