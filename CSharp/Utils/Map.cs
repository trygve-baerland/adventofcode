using System.Text;
using AoC.Y2023;

namespace AoC.Utils;

internal record class Map<T>( T[][] Data )
where T : struct, IEquatable<T>
{
    public int Height => Data.Length;
    public int Width => Data[0].Length;
    public T this[Node2D<int> node]
    {
        get => Data[node.X][node.Y];
        set {
            Data[node.X][node.Y] = value;
        }
    }

    public virtual bool Contains( Node2D<int> node ) =>
        node.X >= 0 && node.X < Height && node.Y >= 0 && node.Y < Width;

    public IEnumerable<Node2D<int>> RowCoordinates( int i ) =>
        Enumerable.Range( 0, Width ).Select( j => new Node2D<int>( i, j ) );

    public IEnumerable<IEnumerable<Node2D<int>>> AllRows() =>
        Enumerable.Range( 0, Height ).Select( i => RowCoordinates( i ) );
    public IEnumerable<Node2D<int>> Coordinates() => AllRows().Flatten();


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

    public IEnumerable<Node2D<int>> Where( Func<T, bool> pred ) =>
        Coordinates().Where( node => pred( this[node] ) );

    public Map<TResult> Select<TResult>( Func<T, TResult> func )
    where TResult : struct, IEquatable<TResult>
    {
        return new Map<TResult>(
            Data.Select(
                row => row.Select( func ).ToArray()
            ).ToArray()
        );
    }
}

internal record class CharMap( char[][] Data ) : Map<char>( Data )
{

    public static CharMap FromFile( string path ) =>
        new( path.GetLines().Select( line => line.ToCharArray() ).ToArray() );

    public override string ToString()
    {
        return base.ToString();
    }

    public CharMap With( char c, Node2D<int> pos )
    {
        var result = new CharMap( Data.Select( row => row.Select( col => col ).ToArray() ).ToArray() );
        if ( Contains( pos ) ) result.Data[pos.X][pos.Y] = c;
        return result;
    }

}

internal record class IntMap( int[][] Data ) : Map<int>( Data )
{

}
