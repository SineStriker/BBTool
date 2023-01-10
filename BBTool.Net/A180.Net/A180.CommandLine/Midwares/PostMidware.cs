using System.CommandLine.Builder;
using System.CommandLine.Help;

namespace A180.CommandLine.Midwares;

/// <summary>
/// 最后安装
/// </summary>
public class PostMidware : BaseMidware
{
    public PostMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    public override void Setup()
    {
        // 没有命令行参数直接输出帮助信息
        Builder.AddMiddleware(async (context, next) =>
        {
            if (context.ParseResult.Tokens.Count == 0)
            {
                var helpBld = new HelpBuilder(context.LocalizationResources, Console.WindowWidth);
                helpBld.Write(context.ParseResult.CommandResult.Command, Console.Out);
            }
            else
            {
                await next(context);
            }
        });

        // 添加默认选项
        Builder.UseVersionOption("-v", "--version");
        Builder.UseHelp("-h", "--help");
        Builder.UseDefaults();
    }
}