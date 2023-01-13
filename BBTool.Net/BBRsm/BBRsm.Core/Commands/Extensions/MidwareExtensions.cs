using System.CommandLine.Builder;
using A180.CommandLine.Midwares;
using System.CommandLine;

namespace BBRsm.Core.Commands.Extensions;

public static class MidwareExtensions
{
    public static CommandLineBuilder AddPort(this CommandLineBuilder builder, Option<int>? opt = null)
    {
        var option = opt ?? new Option<int>("--port", "服务端口号");
        return OptionMidware<int>.CreateGlobal(builder, option,
            (hasOption, optionHandler) =>
            {
                if (hasOption)
                {
                    Rsm.ServerPort = optionHandler.Invoke();
                }

                return true;
            }
        ).Setuped();
    }
}