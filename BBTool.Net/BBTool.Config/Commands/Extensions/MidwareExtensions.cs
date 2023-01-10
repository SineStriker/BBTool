using System.CommandLine.Builder;
using A180.CommandLine.Midwares;
using System.CommandLine;
using A180.CoreLib.Kernel.Extensions;

namespace BBTool.Config.Commands.Extensions;

public static class MidwareExtensions
{
    public static CommandLineBuilder AddCookiePath(this CommandLineBuilder builder)
    {
        var option = new Option<bool>("--debug", "调试模式");
        OptionMidware<bool>.Create(builder, option,
            (hasOption, optionHandler) =>
            {
                if (hasOption)
                {
                    MessageTool.DebugMode = true;
                    Core.Global.EnableDebug = true;
                }

                return true;
            }
        ).Setup();
        return builder;
    }

    public static CommandLineBuilder AddGlobal(this CommandLineBuilder builder)
    {
        var option =
            new Option<FileInfo>("--cookie",
                () => new FileInfo(MessageTool.CookiePath.FileName()), "设置Cookie路径"
            )
            {
                ArgumentHelpName = "file",
            };

        OptionMidware<FileInfo>.Create(builder, option,
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
        ).Setup();
        return builder;
    }

    public static CommandLineBuilder AddPost(this CommandLineBuilder builder)
    {
        new PostMidware(builder).Setup();

        return builder;
    }
}