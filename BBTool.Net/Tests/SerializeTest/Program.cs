// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Text.Json;
using A180.CoreLib.Text;

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
        Console.WriteLine(AJson.Serialize(new Derived()));
        return 0;
    }
}