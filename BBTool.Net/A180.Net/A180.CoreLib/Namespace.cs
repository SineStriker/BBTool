using System.Runtime.CompilerServices;
using System.Text;

namespace A180.CoreLib;

public static class Namespace
{
    public static RootObject Instance { get; } = new();

    public class RootObject
    {
    }

    [ModuleInitializer]
    internal static void Init()
    {
        // 添加终端编码信息
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}