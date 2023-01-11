// See https://aka.ms/new-console-template for more information

using A180.Network.Http;

namespace A180;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("你喜欢打CF吗？");
        Console.WriteLine(await HttpNew.Get("https://www.bilibili.com/"));
        return 0;
    }
}