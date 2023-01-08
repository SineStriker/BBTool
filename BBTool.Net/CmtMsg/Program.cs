// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using BBDown;
using BBDown.Core;

namespace CmtMsg;

public static class Program
{
    private static volatile bool RequestExit = false;

    private static volatile bool AcceptExit = false;

    private static volatile bool MissionSuccess = false;

    public static int MainRoutine(string[] args)
    {
        // 解析命令行参数

        // 没有命令行参数
        if (!args.Any())
        {
            if (File.Exists(APP_DATA_PATH))
            {
                recover = true;
            }
            else
            {
                ShowHelp();
                return 0;
            }
        }
        
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
                appData = JsonSerializer.Deserialize<AppData>(File.ReadAllText(APP_DATA_PATH),
                    Sys.UnicodeJsonSerializeOption());
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

            if (get_timeout != appData.GetInterval)
            {
                Logger.Log("使用新设置的获取时间间隔");
                appData.GetInterval = get_timeout;
            }

            if (msg_timeout != appData.MessageInterval)
            {
                Logger.Log("使用新设置的获取时间间隔");
                appData.MessageInterval = msg_timeout;
            }
        }
        else
        {
            appData = new AppData();

            appData.GetInterval = get_timeout;
            appData.MessageInterval = msg_timeout;
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
            appData.CommentCountInfo = Video.GetCommentCount(avid);
            if (appData.CommentCountInfo.Root < 0 || appData.CommentCountInfo.Total < 0)
            {
                Logger.LogError("无法获取评论数");
                return 0;
            }
        }

        Logger.LogDebug($"AV号：{appData.VideoInfo.Avid}");

        Logger.LogColor($"作者：{appData.VideoInfo.Uploader}");
        Logger.LogColor($"发布日期：{appData.VideoInfo.PublishTime}");
        Logger.LogColor($"标题：{appData.VideoInfo.Title}");
        Logger.LogColor($"分区：{appData.VideoInfo.Category}");
        Logger.LogColor($"评论数：{appData.CommentCountInfo.Total}");
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

            if (!MissionSuccess)
            {
                File.WriteAllText(APP_DATA_PATH, JsonSerializer.Serialize(appData, Sys.UnicodeJsonSerializeOption()));
            }
            else
            {
                var info = new FileInfo(APP_DATA_PATH);
                if (info.Exists)
                {
                    info.Delete();
                }
            }
        };
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // true: 不导致退出；false: 会导致退出
            Console.WriteLine("Ctrl+C中断被触发，中止任务。");
        };

        // 依次获取根评论信息
        Logger.Log(
            $"获取根评论区所有用户信息，每{appData.GetInterval}毫秒获取一页，每页{AppData.NumPerPage}个，总共{appData.CommentCountInfo.Root}个...");
        if (!appData.Comments.Over)
        {
            var total = appData.CommentCountInfo.Root;
            AppData.CommentList commentList = appData.Comments;

            var bar = new ProgressBar(0, total);
            bool task1Res = Task.Run(() =>
                {
                    var list = commentList.Values;
                    while (list.Count < total)
                    {
                        var comments = Video.GetRootComments(appData.VideoInfo.Avid, AppData.NumPerPage,
                            (int)((double)list.Count / AppData.NumPerPage) + 1);
                        if (comments == null)
                        {
                            return false;
                        }

                        list.AddRange(comments);
                        bar.Report(list.Count);

                        // 避免发送请求太快，设置延时
                        Thread.Sleep((int)appData.GetInterval);

                        if (comments.Count < AppData.NumPerPage)
                        {
                            break;
                        }
                    }

                    commentList.Over = true;
                    return true;
                }
            ).Result;
            bar.Dispose();
            if (!task1Res)
            {
                Logger.LogError("获取根评论区用户信息失败");
                return 0;
            }
        }

        Logger.Log($"获取根评论区用户信息完成");

        var subCommentTotal = appData.CommentCountInfo.Total - appData.Comments.Values.Count;
        var subCommentIndex = 0;

        // 依次获取副评论区信息
        Logger.Log($"获取副评论区所有用户信息，每{appData.GetInterval}毫秒获取一页，每页{AppData.NumPerPage}个，总共{subCommentTotal}个...");
        foreach (var item in appData.Comments.Values)
        {
            // 如果没有评论
            if (item.Count == 0)
            {
                continue;
            }

            // 如果有评论
            var total = item.Count;
            AppData.CommentList commentList;
            if (!appData.SubComments.TryGetValue(item.Id, out commentList))
            {
                commentList = new AppData.CommentList();
                appData.SubComments.Add(item.Id, commentList);
            }

            // 如果已经获取完了
            if (commentList.Over)
            {
                continue;
            }

            var bar = new ProgressBar(subCommentIndex, subCommentIndex + total);
            bool task2Res = Task.Run(() =>
                {
                    var list = commentList.Values;
                    while (list.Count < total)
                    {
                        var comments = Video.GetSubComments(appData.VideoInfo.Avid, item.Id, AppData.NumPerPage,
                            (int)((double)list.Count / AppData.NumPerPage) + 1);
                        if (comments == null)
                        {
                            return false;
                        }

                        list.AddRange(comments);
                        bar.Report(subCommentIndex + list.Count);

                        // 避免发送请求太快，设置延时
                        Thread.Sleep((int)appData.GetInterval);

                        if (comments.Count < AppData.NumPerPage)
                        {
                            break;
                        }
                    }

                    commentList.Over = true;
                    return true;
                }
            ).Result;
            bar.Dispose();
            if (!task2Res)
            {
                Logger.LogError($"获取副评论区{item.Id}用户信息失败");
                return 0;
            }

            subCommentIndex += commentList.Values.Count;
        }

        Logger.Log($"获取副评论区用户信息完成");

        var users = new List<long>();
        foreach (var item in appData.Comments.Values)
        {
            users.Add(item.UserId);
        }

        foreach (var item in appData.SubComments.Values)
        {
            foreach (var subitem in item.Values)
            {
                users.Add(subitem.UserId);
            }
        }

        // Console.WriteLine(User.SendMessage(userInfo.UserId, 329506771, "."));
        // return 0;

        // 发送消息
        Logger.Log($"向所有用户发送消息，每{appData.MessageInterval}毫秒发送一次...");
        {
            var bar = new ProgressBar(appData.MessageSent, users.Count);

            var task3Res = Task.Run(() =>
            {
                int i = appData.MessageSent;
                for (; i < users.Count; ++i)
                {
                    var receiver = users[i];
                    var code = User.SendMessage(userInfo.UserId, receiver, appData.Message);
                    if (code < 0)
                    {
                        // 必须中断的错误
                        Logger.LogError("网络错误");
                        break;
                    }

                    if (code == 21046)
                    {
                        // 频率过高
                        Logger.LogError("发送消息频率过高，如果继续使用此账号，则需要等待较长时间");
                        break;
                    }

                    if (code > 0)
                    {
                        // 记录遇到错误的用户
                        HashSet<long> list;
                        if (!appData.ErrorAttempts.TryGetValue(code, out list))
                        {
                            list = new HashSet<long>();
                            appData.ErrorAttempts.Add(code, list);
                        }

                        list.Add(receiver);
                    }

                    bar.Report(i);
                    appData.MessageSent = i;

                    // 避免发送请求太快，设置延时
                    Thread.Sleep((int)appData.MessageInterval);
                }

                return i;
            });

            task3Res.Wait();

            bar.Dispose();
        }

        if (appData.MessageSent == users.Count)
        {
            // 特殊错误一览
            foreach (var item in appData.ErrorAttempts)
            {
                string err;
                if (SpecialErrors.TryGetValue(item.Key, out err))
                {
                    Logger.LogWarn($"{err}：{string.Join(';', item.Value.Select(item => { return item.ToString(); }))}");
                }
            }

            MissionSuccess = true;
        }

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