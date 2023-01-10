using System.CommandLine.Builder;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace A180.CommandLine.Midwares;

public class OptionMidware<T> : TemplateMidware
{
    public OptionMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    /// <summary>
    /// 创建模板实例，处理函数第一个参数为是否包含该选项，第二个参数为获取值的函数（可能抛出异常）
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="option"></param>
    /// <param name="optionHandler">选项处理函数，true则继续，false则中断</param>
    public static OptionMidware<T> Create(CommandLineBuilder builder, Option<T> option,
        Func<bool, Func<T>, bool> optionHandler)
    {
        return new OptionMidware<T>(builder)
        {
            Work = () =>
            {
                // 全局选项
                builder.Command.AddGlobalOption(option);

                builder.AddMiddleware(async (context, next) =>
                {
                    var parseRes = context.ParseResult;

                    bool hasOption = parseRes.HasOption(option);
                    if (!optionHandler.Invoke(hasOption, () => parseRes.GetValueForOption(option)!))
                    {
                        return;
                    }

                    await next(context);
                });
            }
        };
    }
}