namespace AoC.Utils;

public static class Helpers
{
    public static Action Repeat( Action action, int times )
    {
        return () => Enumerable.Range( 0, times ).ForEach( _ => action() );
    }
}
