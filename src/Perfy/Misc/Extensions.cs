namespace Perfy.Misc;

public static class Extensions
{
    public static string FormatForDisplay<T>(this T obj, Func<T, string> fn) => fn(obj);
}
