using System.CommandLine.Help;
using System.CommandLine.Invocation;

namespace A180.CommandLine.Midwares.Extensions;

public static class Help
{
    public static void ShowHelp(this InvocationContext context)
    {
        var helpBld = new HelpBuilder(context.LocalizationResources, Console.WindowWidth);
        helpBld.Write(context.ParseResult.CommandResult.Command, Console.Out);
    }
}