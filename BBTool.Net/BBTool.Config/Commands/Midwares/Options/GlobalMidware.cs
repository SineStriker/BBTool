using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace BBTool.Config.Commands.Midwares;

public class GlobalMidware : BaseMidware
{
    public Option<bool> Debug = new("--debug", "调试模式");

    public GlobalMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    public override void Setup()
    {
        // 全局选项
        Builder.Command.AddGlobalOption(Debug);

        Builder.AddMiddleware(async (context, next) =>
        {
            if (context.ParseResult.HasOption(Debug))
            {
                MessageTool.DebugMode = true;
                Core.Global.EnableDebug = true;
            }

            await next(context);
        });
    }
}