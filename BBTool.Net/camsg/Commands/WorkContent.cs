using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using System.Text.Json;
using System.Text.RegularExpressions;
using BBDown.Core;
using BBTool.Config;
using BBTool.Core.Entities;
using BBTool.Core.User;
using BBTool.Core.LowLevel;
using Camsg.Tasks;

namespace Camsg.Commands;

public class WorkContent
{
    // 选项
    public Option<FileInfo> Config = new(new[] { "-c", "--config" }, "从配置文件获取参数")
    {
        ArgumentHelpName = "file",
    };

    public Option<string> Message = new(new[] { "-m", "--message" }, "要发送的消息内容");

    public Option<int> T1 = new("-t1", $"指定发送普通请求的时间间隔（毫秒），默认值{AppConfig.DefaultGetTimeout}")
    {
        ArgumentHelpName = "timeout",
    };

    public Option<int> T2 = new("-t2", $"指定发送消息的时间间隔（毫秒），默认值{AppConfig.DefaultMessageTimeout}")
    {
        ArgumentHelpName = "timeout",
    };

    public class WorkOption
    {
        public bool UseConfigFile = false;

        public string VideoId = "";
    }

    // 解析器
    public class Binder : BinderBase<WorkOption>
    {
        public Binder(WorkContent cmd, Action<WorkOption, BindingContext> impl) => (_cmd, _impl) = (cmd, impl);

        protected override WorkOption GetBoundValue(BindingContext bindingContext)
        {
            var opt = new WorkOption();

            var res = bindingContext.ParseResult;

            if (res.HasOption(_cmd.Config))
            {
                var info = res.GetValueForOption(_cmd.Config);

                Global.Config = JsonSerializer.Deserialize<AppConfig>(
                    File.ReadAllBytes(info.FullName),
                    Sys.UnicodeJsonSerializeOption()
                );
                if (Global.Config == null)
                {
                    throw new FormatException($"{info.FullName} 格式不正确");
                }

                opt.UseConfigFile = true;
            }

            if (res.HasOption(_cmd.Message) && !opt.UseConfigFile)
            {
                Global.Config.Message = res.GetValueForOption(_cmd.Message);
            }

            if (res.HasOption(_cmd.T1) && !opt.UseConfigFile)
            {
                Global.Config.GetTimeout = res.GetValueForOption(_cmd.T1);
            }

            if (res.HasOption(_cmd.T2) && !opt.UseConfigFile)
            {
                Global.Config.MessageTimeout = res.GetValueForOption(_cmd.T2);
            }

            if (_impl != null)
            {
                _impl.Invoke(opt, bindingContext);
            }

            return opt;
        }

        protected WorkContent _cmd;

        protected Action<WorkOption, BindingContext> _impl;
    }

    public void Setup(Command command, Action<WorkOption, BindingContext> impl)
    {
        command.Add(Config);
        command.Add(Message);
        command.Add(T1);
        command.Add(T2);

        command.SetHandler(async opt => await Routine(opt), new Binder(this, impl));
    }

    private void RemoveLogDir()
    {
        // 递归删除日志目录
        var dir = new DirectoryInfo(MessageTool.AppLogDir);
        if (dir.Exists)
        {
            dir.Delete(true);
        }
    }

    private async Task Routine(WorkOption opt)
    {
        // 加载 COOKIE
        if (File.Exists(MessageTool.CookiePath))
        {
            Logger.Log("加载本地cookie...");
            MessageTool.Cookie = File.ReadAllText(MessageTool.CookiePath);
        }

        // 检测用户是否登录
        Logger.Log("获取用户信息...");
        UserInfo user;
        {
            var api = new GetInfo();
            user = await api.Send(MessageTool.Cookie);
            if (UserInfo.IsNullOrOff(user))
            {
                Logger.LogWarn("你尚未登录B站账号, 无法进行后续操作");
                return;
            }

            Logger.LogColor($"用户名：{user.UserName}");
            Logger.LogColor($"用户id：{user.Mid}");
        }
        Console.WriteLine();

        // 删除旧的日志
        if (!MessageTool.RecoveryMode)
        {
            RemoveLogDir();
        }

        Directory.CreateDirectory(MessageTool.AppLogDir);
        
        Directory.CreateDirectory(MessageTool.AppHistoryDir);

        // 开始执行任务
        Logger.Log("第一步");

        // 获取视频内容
        var task1 = new GetVideo();
        if (!await task1.Run(opt.VideoId))
        {
            return;
        }

        var videoInfo = task1.Data.VideoInfo;
        var commentInfo = task1.Data.CommentInfo;

        // 检查是否有消息内容
        string message = Global.Config.Message;
        if (string.IsNullOrEmpty(message))
        {
            Logger.LogWarn("缺少消息内容");
            return;
        }

        Console.WriteLine();

        // 全局捕获退出信号
        MessageTool.InstallInterruptFilter();

        Logger.Log("第二步");
        Logger.Log(
            $"获取根评论区所有用户信息，每{Global.Config.GetTimeout}毫秒获取一页，每页{AppConfig.NumPerPage}个，总共{commentInfo.Root}个...");

        // 获取根评论区
        var task2 = new CollectRoot();
        if (!await task2.Run(videoInfo.Avid, commentInfo.Root))
        {
            return;
        }

        var rootComments = task2.Data.Comments;
        var subCount = commentInfo.Total - rootComments.Count;

        Console.WriteLine();

        Logger.Log("第三步");
        Logger.Log($"获取副评论区所有用户信息，每{Global.Config.GetTimeout}毫秒获取一页，每页{AppConfig.NumPerPage}个，总共{subCount}个...");

        // 获取副评论区
        var task3 = new CollectSub();
        if (!await task3.Run(videoInfo.Avid, rootComments))
        {
            return;
        }

        var subCommands = task3.Data.SubComments;

        var users = new List<MidNamePair>();
        {
            foreach (var item in rootComments)
            {
                users.Add(new(item.Mid, item.UserName));
            }

            foreach (var item in subCommands)
            {
                foreach (var subitem in item.Value.Comments)
                {
                    users.Add(new(subitem.Mid, subitem.UserName));
                }
            }
        }

        Console.WriteLine();

        Logger.Log("第四步");
        Logger.Log($"向所有用户发送消息，每{Global.Config.MessageTimeout}毫秒发送一次...");
        Logger.LogColor($"消息内容：{Text.ElideString(message, 10)}");

        // return;

        // 发送消息
        var task4 = new BatchMessage();
        if (!await task4.Run(user.Mid, users, message))
        {
            return;
        }

        // 特殊错误一览
        // ...

        Logger.Log("任务完成，删除所有日志");

        RemoveLogDir();

        // 保存任务历史
        {
            var history = new History
            {
                Avid = videoInfo.Avid,
                Users = users.Select(item => item.Mid).ToList(),
                ErrorAttempts = task4.Data.ErrorAttempts,
            };

            var filename = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".json";
            var path = Path.Combine(MessageTool.AppHistoryDir, filename);
            File.WriteAllText(path, JsonSerializer.Serialize(history, Sys.UnicodeJsonSerializeOption()));
            Logger.Log($"保存本次任务信息到\"{filename}\"中");
        }
    }
}