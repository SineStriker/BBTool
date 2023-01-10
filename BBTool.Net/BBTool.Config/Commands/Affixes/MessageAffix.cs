using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text.Json;
using A180.CommandLine.Affixes;
using A180.CoreLib.Text;
using BBTool.Config.Files;
using BBTool.Core;

namespace BBTool.Config.Commands.Affixes;

public class MessageAffix<T> : BaseAffix where T : MessageConfig
{
    // 选项
    public Option<FileInfo> Config = new(new[] { "-c", "--config" }, "从配置文件获取参数")
    {
        ArgumentHelpName = "file",
    };

    public Option<string> Message = new(new[] { "-m", "--message" }, "要发送的消息内容");

    public Option<int> T1 = new("-t1", $"指定发送普通请求的时间间隔（毫秒），默认值{MessageConfig.DefaultGetTimeout}")
    {
        ArgumentHelpName = "timeout",
    };

    public Option<int> T2 = new("-t2", $"指定发送消息的时间间隔（毫秒），默认值{MessageConfig.DefaultMessageTimeout}")
    {
        ArgumentHelpName = "timeout",
    };

    // 属性
    public bool UseConfigFile { get; set; } = false;

    public MessageAffix(Command command) : base(command)
    {
    }

    public override void Setup()
    {
        Command.Add(Config);
        Command.Add(Message);
        Command.Add(T1);
        Command.Add(T2);
    }

    public override void ResolveResult(InvocationContext context)
    {
        var res = context.ParseResult;

        if (res.HasOption(Config))
        {
            var info = res.GetValueForOption(Config);

            MessageTool.Config = AJson.Load<T>(info.FullName);
            if (MessageTool.Config == null)
            {
                throw new FormatException($"{info.FullName} 格式不正确");
            }

            UseConfigFile = true;
        }

        if (res.HasOption(Message) && !UseConfigFile)
        {
            MessageTool.Config.Message = res.GetValueForOption(Message)!;
        }

        if (res.HasOption(T1) && !UseConfigFile)
        {
            MessageTool.Config.GetTimeout = res.GetValueForOption(T1);
        }

        if (res.HasOption(T2) && !UseConfigFile)
        {
            MessageTool.Config.MessageTimeout = res.GetValueForOption(T2);
        }
    }
}