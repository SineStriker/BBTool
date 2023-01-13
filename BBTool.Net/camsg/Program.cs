// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using A180.CommandLine.Midwares.Extensions;
using A180.CoreLib.Collections.Extensions;
using A180.CoreLib.Kernel.Extensions;
using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Commands.Extensions;
using BBTool.Config.Files;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Entities;
using Camsg.Commands;
using Camsg.Tasks;

namespace Camsg;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // 设置默认配置信息
        MessageTool.Config = new MessageConfig();

        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro =
            $"{MessageTool.AppName} version {ver.Major}.{ver.Minor}.{ver.Build}, {BBTool.Core.Global.Domain} Comment Area Message Tool";

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

        // 获取视频内容
        var task1 = new GetVideo(1);
        ret = await task1.Run(initDirAction);
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        // 检查是否存在历史任务
        var preUserIds = new HashSet<long>();
        {
            var files = new DirectoryInfo(MessageTool.AppHistoryDir).GetFiles(".", SearchOption.AllDirectories);
            foreach (var info in files)
            {
                try
                {
                    var history = await AJson.LoadAsync<History>(info.FullName);
                    foreach (var id in history.Users)
                    {
                        preUserIds.Add(id);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarn($"读取任务日志\"{info.Name}\"失败");
                }
            }

            if (files.Any())
            {
                Logger.Log($"已读取所有任务日志，已排除{preUserIds.Count}个用户");
            }
        }

        var videoInfo = task1.Data.VideoInfo;
        var commentInfo = task1.Data.CommentInfo;
        string message = Global.Config.Message;

        Console.WriteLine();

        // 全局捕获退出信号
        // MessageTool.InstallInterruptFilter();

        Logger.Log("第二步");
        Logger.Log(
            $"获取根评论区所有用户信息，每{Global.Config.GetTimeout}毫秒获取一页，每页{MessageConfig.NumPerPage}个，总共{commentInfo.Root}个...");

        // 获取根评论区
        var task2 = new CollectRoot(2);
        ret = await task2.Run(videoInfo.Avid, commentInfo.Root);
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        var rootComments = task2.Data.Comments;
        var subCount = commentInfo.Total - rootComments.Count;

        Console.WriteLine();

        Logger.Log("第三步");
        Logger.Log(
            $"获取副评论区所有用户信息，每{Global.Config.GetTimeout}毫秒获取一页，每页{MessageConfig.NumPerPage}个，总共{subCount}个...");

        // 获取副评论区
        var task3 = new CollectSub(3);
        ret = await task3.Run(videoInfo.Avid, rootComments);
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        var subCommands = task3.Data.SubComments;

        // 去重
        List<MidNamePair> users;
        {
            var userMap = new Dictionary<long, MidNamePair>();
            
            int a = 0;
            int b = 0;
            
            var addUser = (CommentInfo info) =>
            {
                if (preUserIds.Contains(info.Mid))
                {
                    Logger.LogDebug($"跳过曾经发送过的用户：{info.Mid}");
                    return;
                }

                if (!userMap.ContainsKey(info.Mid))
                {
                    userMap.Add(info.Mid, new(info.Mid, info.UserName));
                }
                else
                {
                    Logger.LogDebug($"跳过重复用户：{info.Mid}");
                }
            };

            rootComments.ForEach(addUser);
            subCommands.ForEach(item => { item.Value.Comments.ForEach(addUser); });

            users = userMap.Values.ToList();
            
            Logger.Log($"已跳过曾经用户{a}个，本次重复用户{b}个");
        }

        Console.WriteLine();

        Logger.Log("第四步");
        Logger.Log($"向所有用户发送消息，每{Global.Config.MessageTimeout}毫秒发送一次...");
        Logger.LogColor($"消息内容：{message.Elide(10)}");

        // return;

        // 发送消息
        var task4 = new BatchMessage(4);
        ret = await task4.Run(user.Mid, users, message);
        if (ret != 0)
        {
            context.ExitCode = ret;
            return;
        }

        var batchResult = task4.Data;

        // 特殊错误一览
        // ...

        Logger.Log("任务完成，删除所有日志");

        MessageTool.AppLogDir.RmDir();

        // 保存任务历史
        {
            var history = new History
            {
                Avid = videoInfo.Avid,
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