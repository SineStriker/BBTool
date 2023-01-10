using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using A180.CoreLib.Kernel.Extensions;

namespace BBTool.Config.Commands.Midwares;

public class CookieMidware : BaseMidware
{
    public Option<FileInfo> CookiePath =
        new("--cookie", () => new FileInfo(MessageTool.CookiePath.FileName()), "设置Cookie路径")
        {
            ArgumentHelpName = "file",
        };

    public CookieMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    public override void Setup()
    {
        // 全局选项
        Builder.Command.AddGlobalOption(CookiePath);

        Builder.AddMiddleware(async (context, next) =>
        {
            try
            {
                var parseRes = context.ParseResult;
                if (parseRes.HasOption(CookiePath))
                {
                    MessageTool.CookiePath = parseRes.GetValueForOption(CookiePath)!.FullName;
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine("参数错误");
                // return;
            }

            await next(context);
        });
    }
}