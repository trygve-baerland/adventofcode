namespace Day9;

public enum Directions
{
    R,
    L,
    U,
    D
}

public class Node
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public (int, int) Point { get => (X, Y); }

    public void Move((int, int) dx)
    {
        X += dx.Item1;
        Y += dx.Item2;
    }

    public static (int, int) operator -(Node x, Node y)
    {
        return (x.X - y.X, x.Y - y.Y);
    }

    public void MoveTowards(Node head)
    {
        var diff = head - this;
        // We're not adjacent to input if the L-infinity distance between them is greater than 1.
        while (Helpers.Linf(diff) > 1)
        {
            // We crop to moving either -1, 0, or 1 in either direction.
            Move(
                (Math.Sign(diff.Item1), Math.Sign(diff.Item2))
            );
            diff = head - this;
        }
    }
}



public static class Helpers
{
    public static int Linf((int, int) x)
    {
        return Math.Max(
            Math.Abs(x.Item1),
            Math.Abs(x.Item2)
        );
    }
}
