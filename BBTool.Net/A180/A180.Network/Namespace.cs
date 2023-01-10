using System.Runtime.CompilerServices;

namespace A180.Network;

public static class Namespace
{
    public static RootObject Instance { get; } = new();

    public class RootObject
    {
    }

    [ModuleInitializer]
    internal static void Init()
    {
    }
}