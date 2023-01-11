// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using A180.CoreLib.Kernel.Extensions;
using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Commands.Extensions;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Entities;
using Somsg.Commands;
using Somsg.Tasks;

namespace Somsg;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // 设置默认配置信息
        MessageTool.Config = new AppConfig();

        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro =
            $"{MessageTool.AppName} version {ver.Major}.{ver.Minor}.{ver.Build}, {BBTool.Core.Global.Domain} Search and Message Tool";

        Console.WriteLine(intro);
        Console.WriteLine();

        // 配置命令
        var rootCommand = new AppCommand();
        rootCommand.SetRoutine(WorkRoutine);

        var parserBuilder = new CommandLineBuilder(rootCommand);
        parserBuilder.AddCookiePath(); // 添加 Cookie 选项
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
        int ret;

        // 获取用户信息
        var task0 = new CheckUser();
        ret = await task0.Run();
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        var user = task0.Data;

        Console.WriteLine();

        // 为了防止输错命令导致误删，将清理工作延迟到第一个任务日志保存前
        var initDirAction = () =>
        {
            // 删除旧的日志
            if (!MessageTool.RecoveryMode)
            {
                MessageTool.AppLogDir.RmDir();
            }

            MessageTool.AppLogDir.MkDir();
            MessageTool.AppHistoryDir.MkDir();
        };

        // 全局捕获关闭事件
        MessageTool.InstallInterruptFilter();

        // 开始执行任务
        Logger.Log("第一步");
        Logger.Log("准备...");

        // 前期缓存工作
        var task1 = new CacheTask(1);
        ret = await task1.Run(initDirAction);
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        string message = Global.Config.Message;

        Console.WriteLine();

        Logger.Log("第一步");
        Logger.LogColor($"关键词：{Global.KeyWord}");
        Logger.Log($"获取所有搜索结果，每{Global.Config.MessageTimeout}毫秒获取一页...");

        // 搜索
        var task2 = new SearchTask(2);
        ret = await task2.Run();
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        var searchResult = task2.Data;

        // 去重
        List<MidNamePair> users;
        {
            var userMap = new Dictionary<long, MidNamePair>();
            foreach (var item in searchResult.Videos)
            {
                if (!userMap.ContainsKey(item.Mid))
                {
                    userMap.Add(item.Mid, new(item.Mid, item.UserName));
                }
                else
                {
                    Logger.LogDebug($"跳过重复用户：{item.Mid}");
                }
            }

            users = userMap.Values.ToList();
        }

        Console.WriteLine();

        Logger.Log("第二步");
        Logger.Log($"向所有用户发送消息，每{Global.Config.MessageTimeout}毫秒发送一次...");
        Logger.LogColor($"消息内容：{message.Elide(10)}");

        // 发送消息
        var task3 = new BatchMessage(3);
        ret = await task3.Run(user.Mid, users, message);
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        var batchResult = task3.Data;

        // 特殊错误一览
        // ...

        Logger.Log("任务完成，删除所有日志");

        MessageTool.AppLogDir.RmDir();

        // 保存任务历史
        {
            var history = new History
            {
                Avids = searchResult.Videos.Select(item => item.Avid).ToList(),
                Users = users.Select(item => item.Mid).ToList(),
                ErrorAttempts = batchResult.ErrorAttempts,
            };

            var filename = DateTime.Now.ToString(MessageTool.HistoryFileFormat) + ".json";
            var path = Path.Combine(MessageTool.AppHistoryDir, filename);
            await AJson.SaveAsync(path, history);
            Logger.Log($"保存本次任务信息到\"{filename}\"中");
        }
    }
}