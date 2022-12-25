using System.Numerics;
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

    public static T MathMod<T>(T a, T b)
    where T : INumber<T>
    {
        return ((a % b) + b) % b;
    }

    public static T Gfc<T>(T a, T b)
    where T : INumber<T>
    {
        while (b != T.Zero)
        {
            T temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static T Lcm<T>(T a, T b)
    where T : INumber<T>
    {
        return a / Gfc(a, b) * b;
    }
}