// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net;
using A180.CoreLib.Text;
using A180.Network;
using A180.Network.Http;
using BBDown.Core;
using BBRsm.Core;
using BBRsm.Daemon.Commands;
using BBTool.Config;
using BBTool.Config.Commands.Extensions;
using BBTool.Config.Files;

namespace BBRsm.Daemon;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // await SimpleHttpServer.RunExampleServer();
        // return 0;

        // 设置默认配置信息
        MessageTool.Config = new AppConfig();

        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro =
            $"{MessageTool.AppName} version {ver.Major}.{ver.Minor}.{ver.Build}, {BBTool.Core.Global.Domain} Round Search Message Daemon";

        Console.WriteLine(intro);
        Console.WriteLine();

        // 配置命令
        var rootCommand = new AppCommand();
        rootCommand.SetRoutine(WorkRoutine);

        var parserBuilder = new CommandLineBuilder(rootCommand);
        parserBuilder.AddGlobal().AddPost();

        // 开始解析
        var parser = parserBuilder.Build();
        var code = await parser.InvokeAsync(args);

        // 同意主程序域退出
        MessageTool.AcceptExit = 1;

        return code;
    }

    static async Task WorkRoutine(InvocationContext context)
    {
        // 连接数据库
        Logger.Log("连接到Redis数据库...");
        try
        {
            _ = RedisHelper.Coonection;
        }
        catch (Exception e)
        {
            Logger.LogError($"无法连接到Redis服务：{e.Message}");
            context.ExitCode = -1;
            return;
        }

        Logger.Log($"启动服务监听，端口号{Global.ServerPort}...");

        // 启动服务
        Global.Server = new HttpServer($"http://localhost:{Global.ServerPort}");

        // 添加中断
        MessageTool.InstallInterruptFilter();
        Global.Server.Cancelers.Add(() =>
        {
            if (MessageTool.Interrupt > 0)
            {
                Logger.LogWarn("用户中断了服务，正在关闭...");
                return true;
            }

            return false;
        });

        // 开始监听
        await Global.Server.Start(ServerHandler);
    }

    static async Task<bool> ServerHandler(HttpListenerRequest req, HttpListenerResponse resp)
    {
        return true;
    }
}