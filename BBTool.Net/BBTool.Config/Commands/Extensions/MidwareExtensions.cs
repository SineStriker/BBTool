using System.CommandLine.Builder;
using A180.CommandLine.Midwares;
using System.CommandLine;
using A180.CoreLib.Kernel.Extensions;

namespace BBTool.Config.Commands.Extensions;

public static class MidwareExtensions
{
    public static CommandLineBuilder AddGlobal(this CommandLineBuilder builder, Option<bool>? opt = null)
    {
        var option = opt ?? new Option<bool>("--debug", "调试模式");
        return OptionMidware<bool>.CreateGlobal(builder, option,
            (hasOption, optionHandler) =>
            {
                if (hasOption)
                {
                    MessageTool.DebugMode = true;
                    Core.Global.EnableDebug = true;
                }

                return true;
            }
        ).Setuped();
    }

    public static CommandLineBuilder AddCookiePath(this CommandLineBuilder builder, Option<FileInfo>? opt = null)
    {
        var option = opt ??
                     new Option<FileInfo>("--cookie",
                         () => new FileInfo(MessageTool.CookiePath.FileName()), "设置Cookie路径"
                     )
                     {
                         ArgumentHelpName = "file",
                     };

        return OptionMidware<FileInfo>.CreateGlobal(builder, option,
            (hasOption, optionHandler) =>
            {
                if (hasOption)
                {
                    try
                    {
                        MessageTool.CookiePath = optionHandler.Invoke().FullName;
                    }
                    catch (Exception e)
                    {
                        // Console.WriteLine("参数错误");
                        // return false;
                    }
                }

                return true;
            }
        ).Setuped();
    }
}