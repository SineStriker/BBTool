// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using BBTool.Config;
using BBTool.Config.Commands;
using Camsg.Commands;

namespace Camsg;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        
        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro = $"Comment Area Message Tool of Bilibili, version {ver.Major}.{ver.Minor}.{ver.Build}.";
        
        Console.WriteLine(intro);
        Console.WriteLine();

        var rootCommand = new AppCommand();
        var parserBuilder = new ParserBuilder(rootCommand, args);

        // 开始解析
        var parser = parserBuilder.Build();
        var code = await parser.InvokeAsync(args);

        // 同意主程序域退出
        MessageTool.AcceptExit = 1;

        return code;
    }
}