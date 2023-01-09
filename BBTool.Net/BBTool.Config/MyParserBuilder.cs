using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using BBTool.Config.Commands;

namespace BBTool.Config;

public class MyParserBuilder : CommandLineBuilder
{
    public MyParserBuilder(Command command, string[] args, Action<CommandLineBuilder> doFirst = null) : base(command)
    {
        if (doFirst != null)
        {
            doFirst.Invoke(this);
        }

        // 全局选项
        var globalContent = new GlobalContent();
        globalContent.Setup(this);

        // 没有命令行参数直接输出帮助信息
        this.AddMiddleware(async (context, next) =>
        {
            if (args.Length == 0)
            {
                var helpBld = new HelpBuilder(context.LocalizationResources, Console.WindowWidth);
                helpBld.Write(context.ParseResult.CommandResult.Command, Console.Out);
            }
            else
            {
                await next(context);
            }
        });

        this.UseHelp("-h", "--help").UseVersionOption("-v", "--version").UseDefaults();
    }
}