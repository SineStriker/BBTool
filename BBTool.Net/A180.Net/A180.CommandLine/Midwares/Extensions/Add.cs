using System.CommandLine.Builder;

namespace A180.CommandLine.Midwares.Extensions;

public static class Init
{
    public static CommandLineBuilder AddPost(this CommandLineBuilder builder)
    {
        return new PostMidware(builder).Setuped();
    }
}