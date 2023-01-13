using System.CommandLine.Builder;
using System.CommandLine.Help;
using A180.CommandLine.Midwares.Extensions;

namespace A180.CommandLine.Midwares;

/// <summary>
/// 最后安装
/// </summary>
public class PostMidware : BaseMidware
{
    public bool ShowHelpIfNoArgs { get; set; } = true;

    public List<string> PreHelpLines { get; } = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="builder">命令行构造器</param>
    /// <param name="showHelpIfNoArgs">如果为true，添加中间件使得当没有参数时直接输出帮助</param>
    public PostMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    public override void Setup()
    {
        // 没有命令行参数直接输出帮助信息
        Builder.AddMiddleware(async (context, next) =>
        {
            if (context.ParseResult.Tokens.Count == 0 && ShowHelpIfNoArgs)
            {
                if (PreHelpLines.Any())
                {
                    PreHelpLines.ForEach(line => Console.WriteLine(line));
                    Console.WriteLine();
                }

                context.ShowHelp();
            }
            else
            {
                await next(context);
            }
        });

        // 添加默认选项
        Builder.UseVersionOption("-v", "--version");
        Builder.UseHelp("-h", "--help");
        Builder.UseHelp(ctx =>
        {
            if (PreHelpLines.Any())
            {
                ctx.HelpBuilder.CustomizeLayout(
                    _ =>
                        HelpBuilder.Default
                            .GetLayout()
                            // .Skip(1) // Skip the default command description section.
                            .Prepend(
                                _ => PreHelpLines.ForEach(line => Console.WriteLine(line))
                            ));
            }
        });

        Builder.UseDefaults();
    }
}