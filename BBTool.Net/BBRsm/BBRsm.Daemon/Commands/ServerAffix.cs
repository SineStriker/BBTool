using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using BBTool.Config;
using BBTool.Config.Commands.Affixes;

namespace BBRsm.Daemon.Commands;

public class ServerAffix : MessageAffix<AppConfig>
{
    // 选项
    public Option<int> Partition =
        new("-s", $"分区号，默认值{(int)AppConfig.DefaultPartition}（{AppConfig.DefaultPartition.ToString()}）")
        {
            ArgumentHelpName = "tid",
        };

    /// <summary>
    /// 不使用
    /// </summary>
    public Option<int> Order =
        new(new[] { "-r", "--order" }, $"排序方式代号，综合0/最热1/最新2/弹幕3/收藏4/评论5，默认为{(int)AppConfig.DefaultSortOrder}")
        {
            ArgumentHelpName = "code",
        };

    public Option<int> Port =
        new(new[] { "-p", "--port" }, $"监听端口号，默认为{Global.ServerPort}")
        {
            ArgumentHelpName = "port",
        };

    public ServerAffix(Command command) : base(command)
    {
    }

    public override void Setup()
    {
        base.Setup();

        Command.Add(Partition);
        // Command.Add(Order);
        
        Command.Add(Port);
    }

    public override void ResolveResult(InvocationContext context)
    {
        base.ResolveResult(context);

        var res = context.ParseResult;

        if (res.HasOption(Partition) && !UseConfigFile)
        {
            Global.Config.PartitionNum = res.GetValueForOption(Partition);
        }

        if (res.HasOption(Order) && !UseConfigFile)
        {
            Global.Config.SortOrderNum = res.GetValueForOption(Order);
        }
        
        if (res.HasOption(Port))
        {
            Global.ServerPort = res.GetValueForOption(Port);
        }
    }
}