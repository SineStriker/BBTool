using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Text.Json;
using BBDown.Core;
using BBTool.Core.LowLevel;

namespace Camsg.Commands;

public class GenConfigCommand : Command
{
    public static readonly string DefaultOutput = Global.AppName + "_template.json";

    // 命令
    public Command Config = new("config", "生成配置文件");

    // 选项
    public Option<FileInfo> Output =
        new(new[] { "-o" }, () => new FileInfo(DefaultOutput), "指定输出路径")
        {
            ArgumentHelpName = "file",
        };

    public GenConfigCommand() : base("gen", "生成指定的模板文件，默认为配置文件")
    {
        Add(Config);

        Add(Output);

        Config.SetHandler(ConfigRoutine);

        this.SetHandler(ConfigRoutine);
    }

    private async Task ConfigRoutine(InvocationContext context)
    {
        var info = context.ParseResult.GetValueForOption(Output);

        var conf = new AppConfig();
        conf.Message = "你好";

        // 生成默认配置信息后退出
        File.WriteAllText(info.FullName, JsonSerializer.Serialize(conf, Sys.UnicodeJsonSerializeOption(true)));

        Console.WriteLine($"写入默认配置文件到\"{info.Name}\"成功");
    }
}