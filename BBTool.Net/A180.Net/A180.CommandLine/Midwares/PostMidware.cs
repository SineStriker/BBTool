using System.CommandLine.Builder;
using System.CommandLine.Help;

namespace A180.CommandLine.Midwares;

/// <summary>
/// 最后安装
/// </summary>
public class PostMidware : BaseMidware
{
    private readonly bool _showHelpIfNoArgs;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="builder">命令行构造器</param>
    /// <param name="showHelpIfNoArgs">如果为true，添加中间件使得当没有参数时直接输出帮助</param>
    public PostMidware(CommandLineBuilder builder, bool showHelpIfNoArgs = true) : base(builder)
    {
        _showHelpIfNoArgs = showHelpIfNoArgs;
    }

    public override void Setup()
    {
        if (_showHelpIfNoArgs)
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
        }

        // 添加默认选项
        Builder.UseVersionOption("-v", "--version");
        Builder.UseHelp("-h", "--help");
        Builder.UseDefaults();
    }
}