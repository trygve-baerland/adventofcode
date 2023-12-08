namespace Utils;

public static class Math
{
    /// Greatest common factor
    public static long GCF( long a, long b )
    {
        while ( b != 0 )
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// Least common multiple
    public static long LCM( long a, long b )
    {
        return a / GCF( a, b ) * b;
    }
}
