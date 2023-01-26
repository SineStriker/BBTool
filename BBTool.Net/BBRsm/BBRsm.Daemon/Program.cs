// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net;
using System.Text;
using A180.CommandLine.Midwares.Extensions;
using A180.CoreLib.Text.Extensions;
using A180.Network.Http;
using BBDown.Core;
using BBRsm.Core;
using BBRsm.Core.BiliApiImpl;
using BBRsm.Core.RPC;
using BBRsm.Daemon.Commands;
using BBRsm.Daemon.HttpHandlers;
using BBRsm.Daemon.Tasks;
using BBTool.Config;
using BBTool.Config.Commands.Extensions;

namespace BBRsm.Daemon;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // await HttpServer.RunExampleServer();
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
            _ = RedisHelper.Connection;
        }
        catch (Exception e)
        {
            Logger.LogError($"无法连接到Redis服务：{e.Message}");
            context.ExitCode = -1;
            return;
        }

        // 启动服务
        Logger.Log($"启动服务监听，端口号{Rsm.ServerPort}...");

        HttpServer.DisableDebug = true;
        Global.Server = new HttpServer(Rsm.ServerUrl);

        // 试图恢复数据库
        {
            Logger.Log("数据库初始化...");

            var db = RedisHelper.Database;
            var server = RedisHelper.Server;

            // 恢复账户
            Global.RecoverData();
        }

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

        var producer = new Producer().Run();

        Console.WriteLine("000");

        var consumer = new Consumer().Run();

        Console.WriteLine("111");

        // 生产者、消费者开始工作
        // producer.Start();
        Console.WriteLine("222");
        // consumer.Start();

        Console.WriteLine("333");

        // 开始监听
        await Global.Server.Start(ServerHandler);

        await producer;
        await consumer;

        if (Global.LogoutTask.Status == TaskStatus.Running)
        {
            Global.LogoutTask.Wait();
        }
    }

    static async Task<bool> ServerHandler(HttpListenerRequest req, HttpListenerResponse resp)
    {
        var returnBase = (int code, string? message) =>
            new BaseResponse
            {
                Code = code,
                Message = message ?? "不知道如何回复本次请求",
            }.ToJson();

        var respData = returnBase(-1, null);

        if (req.HttpMethod == "POST")
        {
            string content;
            using (var streamReader = new StreamReader(req.InputStream, Encoding.UTF8))
            {
                content = await streamReader.ReadToEndAsync();
            }

            // 解析为基本信息
            BaseRequest reqObj;
            try
            {
                reqObj = content.FromJson<BaseRequest>();
            }
            catch (Exception e)
            {
                goto handleOver;
            }

            if (string.IsNullOrEmpty(reqObj.Command))
            {
                goto handleOver;
            }

            // 获取请求类型
            var command = reqObj.Command;
            switch (command)
            {
                case CommmandProtocol.Get:
                    respData = await GetHandler.Respond(content);
                    break;

                case CommmandProtocol.Set:
                    respData = await SetHandler.Respond(content);
                    break;

                case CommmandProtocol.UserAdd:
                    respData = await UserHandler.AddRespond(content);
                    break;

                case CommmandProtocol.UserRemove:
                    respData = await UserHandler.RemoveRespond(content);
                    break;

                case CommmandProtocol.UserClear:
                    respData = await UserHandler.ClearRespond(content);
                    break;

                case CommmandProtocol.UserAll:
                    respData = await UserHandler.ListRespond(content);
                    break;

                case CommmandProtocol.UserActive:
                    respData = await UserHandler.ActiveRespond(content);
                    break;

                case CommmandProtocol.UserBlocked:
                    respData = await UserHandler.BlockedRespond(content);
                    break;

                case CommmandProtocol.UserExpired:
                    // respData = await UserHandler.ListRespond(content);
                    break;

                case CommmandProtocol.UserReceivers:
                    // respData = await UserHandler.ListRespond(content);
                    break;

                case CommmandProtocol.UserBlackList:
                    // respData = await UserHandler.ListRespond(content);
                    break;

                case CommmandProtocol.ShowVideos:
                    respData = await ShowHandler.VideosRespond(content);
                    break;

                case CommmandProtocol.ShowFails:
                    respData = await ShowHandler.FailsRespond(content);
                    break;

                case CommmandProtocol.Start:
                    respData = await ControlHandler.StartRespond(content);
                    break;

                case CommmandProtocol.Stop:
                    respData = await ControlHandler.StopRespond(content);
                    break;

                case CommmandProtocol.Status:
                    respData = await ControlHandler.StatusRespond(content);
                    break;

                default:
                    respData = returnBase(-1, "意外的请求类型");
                    break;
            }
        }

    handleOver:

        using (var stream = resp.OutputStream)
        {
            await stream.WriteAsync(respData.ToUtf8());
        }

        return true;
    }
}