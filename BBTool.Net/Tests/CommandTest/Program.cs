using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

public static class Program
{
    static async Task Main(string[] args)
    {
        var delayOption = new Option<int>("--delay");
        var messageOption = new Option<string>("--message");

        var rootCommand = new RootCommand("Middleware example");
        rootCommand.Add(delayOption);
        rootCommand.Add(messageOption);

        rootCommand.SetHandler(
            (delayOptionValue, messageOptionValue) => { DoRootCommand(delayOptionValue, messageOptionValue); },
            delayOption, messageOption);

        var commandLineBuilder = new CommandLineBuilder(rootCommand);

        commandLineBuilder.AddMiddleware(async (context, next) =>
        {
            Console.WriteLine($"检测 {context.ParseResult.Directives.Count()}");
            foreach (var token in context.ParseResult.Tokens)
            {
                Console.WriteLine(token.Value);
            }
            if (context.ParseResult.Directives.Contains("just-say-hi"))
            {
                context.Console.WriteLine("Hi!");
            }
            else
            {
                Console.WriteLine("继续");
                await next(context);
            }
        });

        commandLineBuilder.UseDefaults();
        commandLineBuilder.EnableDirectives();
        
        var parser = commandLineBuilder.Build();
        
        await parser.InvokeAsync(args);
    }

    public static void DoRootCommand(int delay, string message)
    {
        Console.WriteLine($"--delay = {delay}");
        Console.WriteLine($"--message = {message}");
    }
}