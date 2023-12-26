using System.Numerics;

namespace AoC.Utils;

public record class CharMap( char[][] Map )
{
    public int Height => Map.Length;
    public int Width => Map[0].Length;
    public char this[Node2D<int> node] => Map[node.X][node.Y];

    public bool Contains( Node2D<int> node ) =>
        node.X >= 0 && node.X < Height && node.Y >= 0 && node.Y < Width;
    public static CharMap FromFile( string path ) =>
        new( path.GetLines().Select( line => line.ToCharArray() ).ToArray() );

    public IEnumerable<Node2D<int>> Coordinates() =>
        Enumerable.Range( 0, Height )
        .SelectMany( i =>
            Enumerable.Range( 0, Width )
            .Select( j => new Node2D<int>( i, j ) )
        );

}
