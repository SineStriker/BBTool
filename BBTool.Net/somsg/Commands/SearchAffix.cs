using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using BBTool.Config;
using BBTool.Config.Commands.Affixes;

namespace Somsg.Commands;

public class SearchAffix : MessageAffix<AppConfig>
{
    // 选项
    public Option<int> Partition = new("-s", $"分区号，默认值{(int)AppConfig.DefaultPartition}（{AppConfig.DefaultPartition.ToString()}）")
    {
        ArgumentHelpName = "tid",
    };

    public Option<int> Order =
        new(new[] { "-r", "--order" }, $"排序方式代号，综合0/最热1/最新2/弹幕3/收藏4/评论5，默认为{(int)AppConfig.DefaultSortOrder}")
        {
            ArgumentHelpName = "code",
        };

    public SearchAffix(Command command) : base(command)
    {
    }

    public override void Setup()
    {
        base.Setup();

        Command.Add(Partition);
        Command.Add(Order);
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
    }
}