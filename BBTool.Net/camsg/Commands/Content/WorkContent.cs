using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using System.Text.Json;
using BBDown.Core;
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

    private async Task Routine(WorkOption opt)
    {
        // 加载 COOKIE
        if (File.Exists(Global.CookiePath))
        {
            Logger.Log("加载本地cookie...");
            Global.Cookie = File.ReadAllText(Global.CookiePath);
        }

        // 检测用户是否登录
        Logger.Log("获取用户信息...");
        UserInfo user;
        {
            var api = new GetInfo();
            user = await api.Send(Global.Cookie);
            if (UserInfo.IsNullOrOff(user))
            {
                Logger.LogWarn("你尚未登录B站账号, 无法进行后续操作");
                return;
            }

            Logger.LogColor($"用户名：{user.UserName}");
            Logger.LogColor($"用户id：{user.Mid}");
        }

        // 检查配置信息
        if (string.IsNullOrEmpty(Global.Config.Message))
        {
            Logger.LogWarn("没有消息内容");
            return;
        }

        Console.WriteLine();

        // 开始执行任务
        Logger.Log("第一步");
        var task1 = new GetVideo();
        if (!await task1.Run(opt.VideoId))
        {
            return;
        }

        Console.WriteLine();

        Logger.Log("第二步");
    }
}