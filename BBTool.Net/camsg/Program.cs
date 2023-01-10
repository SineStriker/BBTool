// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using A180.CoreLib.Kernel;
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
        // 获取用户信息
        var task0 = new CheckUser();
        if (!await task0.Run())
        {
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
        if (!await task1.Run(initDirAction))
        {
            return;
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
        if (!await task2.Run(videoInfo.Avid, commentInfo.Root))
        {
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
        if (!await task3.Run(videoInfo.Avid, rootComments))
        {
            return;
        }

        var subCommands = task3.Data.SubComments;

        // 去重
        List<MidNamePair> users;
        {
            var userMap = new Dictionary<long, MidNamePair>();
            foreach (var item in rootComments)
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

            foreach (var item in subCommands)
            {
                foreach (var subitem in item.Value.Comments)
                {
                    if (!userMap.ContainsKey(subitem.Mid))
                    {
                        userMap.Add(subitem.Mid, new(subitem.Mid, subitem.UserName));
                    }
                    else
                    {
                        Logger.LogDebug($"跳过重复用户：{subitem.Mid}");
                    }
                }
            }

            users = userMap.Values.ToList();
        }

        Console.WriteLine();

        Logger.Log("第四步");
        Logger.Log($"向所有用户发送消息，每{Global.Config.MessageTimeout}毫秒发送一次...");
        Logger.LogColor($"消息内容：{message.Elide(10)}");

        // return;

        // 发送消息
        var task4 = new BatchMessage(4);
        if (!await task4.Run(user.Mid, users, message))
        {
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