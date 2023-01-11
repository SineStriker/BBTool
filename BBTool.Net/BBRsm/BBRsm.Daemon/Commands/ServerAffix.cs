using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using BBTool.Config;
using BBTool.Config.Commands.Affixes;

namespace BBRsm.Daemon.Commands;

public class ServerAffix : MessageAffix<AppConfig>
{
    // 选项
    public readonly Option<string> KeyWord = new(new[] { "-k", "--keyword" }, "搜索关键词");

    public readonly Option<int> Partition =
        new(new[] { "-s", "--section" },
            $"分区号，默认值{(int)AppConfig.DefaultPartition}（{AppConfig.DefaultPartition.ToString()}）")
        {
            ArgumentHelpName = "tid",
        };

    public readonly Option<int> BlockTimeout =
        new("--block-timeout", $"指定高频发送消息账户的睡眠时间（毫秒），默认值{AppConfig.DefaultBlockTimeout}")
        {
            ArgumentHelpName = "timeout",
        };

    public readonly Option<int> SearchTimeout =
        new("--search-timeout", $"指定两次执行搜索的时间间隔（毫秒），默认值{AppConfig.DefaultSearchTimeout}")
        {
            ArgumentHelpName = "timeout",
        };

    public readonly Option<int> WaitTimeout =
        new("--wait-timeout", $"指定因先决条件不足而循环等待的时间（毫秒），默认值{AppConfig.DefaultWaitTimeout}")
        {
            ArgumentHelpName = "timeout",
        };

    public readonly Option<int> Port =
        new(new[] { "-p", "--port" }, $"监听端口号，默认值{Global.ServerPort}")
        {
            ArgumentHelpName = "port",
        };

    public ServerAffix(Command command) : base(command)
    {
    }

    public override void Setup()
    {
        Command.Add(Config);
        Command.Add(KeyWord);
        Command.Add(Message);
        Command.Add(T1);
        Command.Add(T2);
        Command.Add(Partition);
        Command.Add(BlockTimeout);
        Command.Add(SearchTimeout);
        Command.Add(WaitTimeout);

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

        if (res.HasOption(BlockTimeout) && !UseConfigFile)
        {
            Global.Config.BlockTimeout = res.GetValueForOption(BlockTimeout);
        }

        if (res.HasOption(SearchTimeout) && !UseConfigFile)
        {
            Global.Config.SearchTimeout = res.GetValueForOption(SearchTimeout);
        }

        if (res.HasOption(WaitTimeout) && !UseConfigFile)
        {
            Global.Config.WaitTimeout = res.GetValueForOption(WaitTimeout);
        }

        if (res.HasOption(Port))
        {
            Global.ServerPort = res.GetValueForOption(Port);
        }
    }
}