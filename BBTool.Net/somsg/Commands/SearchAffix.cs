using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using BBTool.Config;
using BBTool.Config.Commands.Affixes;

namespace Somsg.Commands;

public class SearchAffix : MessageAffix<AppConfig>
{
    // 选项
    public Option<int> S1 = new("-s1", $"分区号，默认值{(int)AppConfig.Partition.动物圈}({AppConfig.Partition.动物圈.ToString()})")
    {
        ArgumentHelpName = "tid1",
    };

    public Option<int> S2 = new("-s2", $"子分区号，默认值{(int)AppConfig.Partition.喵星人}({AppConfig.Partition.喵星人.ToString()})")
    {
        ArgumentHelpName = "tid2",
    };

    public SearchAffix(Command command) : base(command)
    {
    }

    public override void Setup()
    {
        base.Setup();

        Command.Add(S1);
        Command.Add(S2);
    }

    public override void ResolveResult(InvocationContext context)
    {
        base.ResolveResult(context);

        var res = context.ParseResult;

        if (res.HasOption(S1) && !UseConfigFile)
        {
            Global.Config.Partition1 = res.GetValueForOption(S1);
        }

        if (res.HasOption(S2) && !UseConfigFile)
        {
            Global.Config.Partition2 = res.GetValueForOption(S2);
        }
    }
}