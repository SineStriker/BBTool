// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using A180.CommandLine.Midwares;
using A180.CommandLine.Midwares.Extensions;
using BBRsm.Controller.Commands;
using BBRsm.Core.Commands.Extensions;
using BBTool.Config;
using BBTool.Config.Commands.Extensions;

namespace BBRsm.Controller;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // 设置默认配置信息
        // MessageTool.Config = new MessageConfig();

        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro =
            $"{MessageTool.AppName} version {ver.Major}.{ver.Minor}.{ver.Build}, {BBTool.Core.Global.Domain} Round Search Message Controller";

        // Console.WriteLine(intro);
        // Console.WriteLine();

        // 配置命令
        var rootCommand = new AppCommand();

        var parserBuilder = new CommandLineBuilder(rootCommand);
        parserBuilder.AddPort();
        parserBuilder.AddGlobal();

        var postMidware = new PostMidware(parserBuilder);
        postMidware.PreHelpLines.Add(intro);
        postMidware.Setup();

        // 开始解析
        var parser = parserBuilder.Build();
        var code = await parser.InvokeAsync(args);

        // 同意主程序域退出
        MessageTool.AcceptExit = 1;

        return code;
    }
}