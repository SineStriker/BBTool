﻿// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BBDown;
using BBDown.Core;
using CmtMsg;

public static class Program
{
    public readonly static string APP_NAME = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;

    public readonly static string APP_DIR = Path.GetDirectoryName(Environment.ProcessPath)!;

    public readonly static string APP_DATA_FILE_NAME = "CmtMsg.data.json";

    public readonly static string APP_DATA_PATH = Path.Combine(APP_DIR, APP_DATA_FILE_NAME);

    private static volatile bool RequestExit = false;

    private static volatile bool AcceptExit = false;

    private static volatile bool CacheReadOver = false;

    public class AppData
    {
        public class CommentList
        {
            public List<Video.CommentInfo> Values { get; set; } = new List<Video.CommentInfo>();
            public bool Over { get; set; } = false;
        }

        public static readonly int NumPerPage = 20;
        public long Interval { get; set; } = 1000;
        public string VideoId { get; set; } = "";
        public string Message { get; set; } = "";
        public Video.VideoInfo VideoInfo { get; set; } = new Video.VideoInfo();
        public int CommentCount { get; set; } = -1;
        public int RootCommentCount { get; set; } = -1;
        public CommentList Comments { get; set; } = new CommentList();

        public List<CommentList> SubComments { get; set; } = new List<CommentList>();

        public bool IsValid()
        {
            if (!VideoId.Any() || !Message.Any())
            {
                return false;
            }

            if (VideoInfo.Avid < 0 || CommentCount < 0 || RootCommentCount < 0)
            {
                return false;
            }

            return true;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("简介：");
        Console.WriteLine("  {0}", "给指定的B站视频评论区用户批量发送私信。");
        Console.WriteLine();

        Console.WriteLine("用法：");
        Console.WriteLine("  {0}", $"{APP_NAME} <视频id> [选项] [消息内容]");
        Console.WriteLine();

        Console.WriteLine("选项：");
        Console.WriteLine("  {0,-15}{1}", "-f <file>", "使用消息文件，不需要指定消息内容");
        Console.WriteLine("  {0,-15}{1}", "-t", "设置消息发送时间间隔（毫秒），默认值1000");
        Console.WriteLine("  {0,-15}{1}", "--debug", "输出调试信息");
        Console.WriteLine("  {0,-15}{1}", "--recover", "尝试恢复上一次的事务");
        Console.WriteLine("  {0,-15}{1}", "-h, --help", "显示帮助");
    }

    public static int MainRoutine(string[] args)
    {
        // 没有命令行参数
        if (!args.Any())
        {
            ShowHelp();
            return 0;
        }

        // 解析命令行参数
        string msg_path = "";
        long interval = 1000;
        bool recover = false;

        var positionalArgs = new List<string>();

        for (int i = 0; i < args.Length; ++i)
        {
            string arg = args[i];
            string nextArg = i < args.Length - 1 ? args[i + 1] : "";
            if (arg == "-f")
            {
                // 文件
                msg_path = nextArg;
                i++;
            }
            else if (arg == "-t")
            {
                // 设置消息发送间隔
                try
                {
                    interval = Int64.Parse(nextArg);
                }
                catch (Exception e)
                {
                    Console.Write($"参数错误");
                    return -1;
                }

                i++;
            }
            else if (arg == "--debug")
            {
                // 输出调试信息
                Config.DEBUG_LOG = true;
            }
            else if (arg == "--recover")
            {
                // 恢复模式
                recover = true;
            }
            else if (arg == "-h" || arg == "--help")
            {
                // 显示帮助
                ShowHelp();
                return 0;
            }
            else if (!arg.StartsWith("-"))
            {
                positionalArgs.Add(arg);
            }
        }

        string vid = positionalArgs.Any() ? positionalArgs.First() : "";
        string msg = positionalArgs.Count > 1 ? positionalArgs[1] : "";
        if (msg_path.Any())
        {
            try
            {
                msg = File.ReadAllText(msg_path, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Console.WriteLine($"读取文件\"{msg_path}\"错误");
                return -1;
            }
        }

        if (!recover)
        {
            if (!vid.Any())
            {
                Console.WriteLine("没有指定视频id");
                return -1;
            }

            if (!msg.Any())
            {
                Console.WriteLine("没有指定消息内容");
                return -1;
            }
        }

        // 获取BBDown缓存
        if (string.IsNullOrEmpty(Config.COOKIE) && File.Exists(Path.Combine(APP_DIR, "BBDown.data")))
        {
            Logger.Log("加载本地cookie...");
            Logger.LogDebug("文件路径：{0}", Path.Combine(APP_DIR, "BBDown.data"));
            Config.COOKIE = File.ReadAllText(Path.Combine(APP_DIR, "BBDown.data"));
        }

        // 检测用户信息
        Logger.Log("检测用户信息...");

        var userInfo = User.GetUserInfo();
        if (!userInfo.IsLogin)
        {
            Logger.LogError("你尚未登录B站账号, 无法进行后续操作，请先使用BBDown登录");
            return 0;
        }

        Logger.LogColor($"用户名：{userInfo.UserName}");
        Logger.LogColor($"用户id：{userInfo.UserId}");
        Console.WriteLine();

        // 正文
        AppData appData = null;

        if (recover)
        {
            // 检测缓存
            try
            {
                appData = JsonSerializer.Deserialize<AppData>(File.ReadAllText(APP_DATA_PATH));
                if (!appData.IsValid())
                {
                    appData = null;
                    Logger.LogWarn("缓存文件有误，忽略");
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn("无法读取缓存文件");
            }

            if (appData == null)
            {
                Logger.LogError("无法恢复上一次事务");
                return 0;
            }

            Logger.Log("恢复上一次的事务成功");

            // 查看是否设置了新的参数
            if (msg.Any())
            {
                Logger.Log("使用新设置的消息内容");
                appData.Message = msg;
            }

            if (interval != appData.Interval)
            {
                Logger.Log("使用新设置的时间间隔");
                appData.Interval = interval;
            }
        }
        else
        {
            appData = new AppData();

            appData.Interval = interval;
            appData.VideoId = vid;
            appData.Message = msg;
        }

        Logger.LogColor($"视频id：{appData.VideoId}");
        Logger.LogColor($"消息内容：{appData.Message}");
        Console.WriteLine();

        if (!appData.IsValid())
        {
            // 获取视频信息
            Logger.Log("获取视频信息...");

            // 获取视频信息
            if (vid.ToLower().StartsWith("av") || vid.ToLower().StartsWith("bv"))
            {
                appData.VideoInfo = Video.GetVideoInfo(vid);
                if (appData.VideoInfo.Avid < 0)
                {
                    Logger.LogError("未获取到正确的视频信息");
                    return 0;
                }
            }
            else
            {
                Logger.LogError("非法的视频id");
                return 0;
            }

            var avid = appData.VideoInfo.Avid;

            // 获取评论总数
            appData.CommentCount = Video.GetCommentCount(avid);
            if (appData.CommentCount < 0)
            {
                Logger.LogError("无法获取评论数");
                return 0;
            }

            // 获取根评论总数
            appData.RootCommentCount = Video.GetRootCommentCount(avid);
            if (appData.RootCommentCount < 0)
            {
                Logger.LogError("无法获取根评论数");
                return 0;
            }
        }

        Logger.LogDebug($"AV号：{appData.VideoInfo.Avid}");

        Logger.LogColor($"作者：{appData.VideoInfo.Uploader}");
        Logger.LogColor($"发布日期：{appData.VideoInfo.PublishTime}");
        Logger.LogColor($"标题：{appData.VideoInfo.Title}");
        Logger.LogColor($"分区：{appData.VideoInfo.Category}");
        Logger.LogColor($"评论数：{appData.CommentCount}");
        Console.WriteLine();

        // 添加退出事件
        AppDomain appd = AppDomain.CurrentDomain;
        appd.ProcessExit += (s, e) =>
        {
            RequestExit = true;
            while (!AcceptExit)
            {
                // 等待主线程同意退出
            }

            File.WriteAllText(APP_DATA_PATH, JsonSerializer.Serialize(appData));
        };
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // true: 不导致退出；false: 会导致退出
            Console.WriteLine("Ctrl+C中断被触发，中止任务。");
        };

        // 依次获取根评论信息
        if (!appData.Comments.Over)
        {
            Logger.Log($"获取所有根评论信息，每页{AppData.NumPerPage}个，总共{appData.RootCommentCount}个...");

            var bar = new ProgressBar(0, appData.RootCommentCount);
            bool task1Res = Task.Run(() =>
                {
                    var list = appData.Comments.Values;
                    while (list.Count < appData.RootCommentCount)
                    {
                        var comments = Video.GetRootComments(appData.VideoInfo.Avid, AppData.NumPerPage,
                            (int)((double)list.Count / AppData.NumPerPage) + 1);
                        if (comments == null)
                        {
                            return false;
                        }

                        list.AddRange(comments);
                        bar.Report(list.Count);

                        if (comments.Count < AppData.NumPerPage)
                        {
                            break;
                        }

                        // 避免发送请求太快，设置延时
                        Thread.Sleep((int)appData.Interval);
                    }

                    appData.Comments.Over = true;
                    return true;
                }
            ).Result;
            bar.Dispose();
            if (!task1Res)
            {
                Logger.LogError("获取根评论信息失败");
                return 0;
            }
        }
        Logger.Log($"获取根评论区信息完成");
        
        // 依次获取副评论区信息
        

        return 0;
    }

    public static int Main(string[] args)
    {
        int code = MainRoutine(args);

        // 同意退出
        AcceptExit = true;

        return code;
    }
}