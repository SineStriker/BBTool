using System.CommandLine.Builder;
using BBTool.Config.Commands.Midwares;

namespace BBTool.Config.Commands.Extensions;

public static class MidwareExtensions
{
    public static CommandLineBuilder AddCookiePath(this CommandLineBuilder builder)
    {
        new CookieMidware(builder).Setup();

        return builder;
    }
    public static CommandLineBuilder AddGlobal(this CommandLineBuilder builder)
    {
        new GlobalMidware(builder).Setup();

        return builder;
    }

    public static CommandLineBuilder AddPost(this CommandLineBuilder builder)
    {
        new PostMidware(builder).Setup();

        return builder;
    }
}