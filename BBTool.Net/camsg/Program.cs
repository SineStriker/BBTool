// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Camsg.Commands;

namespace Camsg;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro = $"Comment Area Message Tool of Bilibili, version {ver.Major}.{ver.Minor}.{ver.Build}.";

        Console.WriteLine(intro);
        Console.WriteLine();

        var rootCommand = new AppCommand();
        var parserBuilder = new CommandLineBuilder(rootCommand);

        // 全局选项
        var globalContent = new GlobalContent();
        globalContent.Setup(parserBuilder);

        // 没有命令行参数直接输出帮助信息
        parserBuilder.AddMiddleware(async (context, next) =>
        {
            if (args.Length == 0)
            {
                var helpBld = new HelpBuilder(context.LocalizationResources, Console.WindowWidth);
                helpBld.Write(context.ParseResult.CommandResult.Command, Console.Out);
            }
            else
            {
                await next(context);
            }
        });

        parserBuilder.UseHelp(new[] { "-h", "--help" });
        parserBuilder.UseVersionOption(new[] { "-v", "--version" });
        parserBuilder.UseDefaults();

        // 开始解析
        var parser = parserBuilder.Build();
        return await parser.InvokeAsync(args);
    }
}