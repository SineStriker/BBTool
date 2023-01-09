// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using BBTool.Core.LowLevel;

public static class Program
{
    public class Base
    {
        public string Father { get; set; } = "father";
    }

    public class Derived : Base
    {
        public string Child { get; set; } = "child";
    }

    public static int Main(string[] args)
    {
        Console.WriteLine(JsonSerializer.Serialize(new Derived(), Sys.UnicodeJsonSerializeOption()));
        return 0;
    }
}