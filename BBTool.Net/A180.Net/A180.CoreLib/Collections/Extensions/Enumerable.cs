namespace A180.CoreLib.Collections.Extensions;

public static class Enumerable
{
    /// <summary>
    /// 与 List&lt;T&gt;.ForEach 相同
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action.Invoke(item);
        }
    }
}