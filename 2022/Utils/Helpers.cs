namespace Utils;

public static class Helpers
{
    public static void Repeat(Action action, int times)
    {
        Enumerable.Range(0, times).ForEach(_ => action());
    }

    public static void Repeat(Action<int> action, int times)
    {
        Enumerable.Range(0, times).ForEach(index => action(index));
    }

    public static int MathMod(int a, int b)
    {
        return ((a % b) + b) % b;
    }
}