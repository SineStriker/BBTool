using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace Camsg.Commands;

public class GlobalContent
{
    public Option<bool> Debug = new("--debug", "调试模式");

    public void Setup(CommandLineBuilder commandLineBuilder)
    {
        commandLineBuilder.Command.AddGlobalOption(Debug);

        commandLineBuilder.AddMiddleware(async (context, next) =>
        {
            if (context.ParseResult.HasOption(Debug))
            {
                Global.DebugMode = true;
                BBTool.Core.Global.EnableDebug = true;
            }

            await next(context);
        });
    }
}