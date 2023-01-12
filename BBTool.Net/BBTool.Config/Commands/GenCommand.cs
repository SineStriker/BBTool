using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Text.Json;
using A180.CoreLib.Text;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Files;

namespace BBTool.Config.Commands;

public class GenConfigCommand : Command
{
    public static readonly string DefaultOutput = MessageTool.AppName + "_template.json";

    // 命令
    public readonly Command Config = new("config", "生成配置文件");

    // 选项
    public readonly Option<FileInfo> Output =
        new(new[] { "-o" }, () => new FileInfo(DefaultOutput), "指定输出路径")
        {
            ArgumentHelpName = "file",
        };

    public GenConfigCommand() : base("gen", "生成指定的模板文件，默认为配置文件")
    {
        Add(Config);

        Add(Output);

        Config.SetHandler(ConfigRoutine);

        // 默认生成配置文件
        this.SetHandler(ConfigRoutine);
    }

    private async Task ConfigRoutine(InvocationContext context)
    {
        var info = context.ParseResult.GetValueForOption(Output)!;

        await GenerateConfigFile(info);

        Console.WriteLine($"写入默认配置文件到\"{info.Name}\"成功");
    }

    protected virtual async Task GenerateConfigFile(FileInfo info)
    {
        var conf = new MessageConfig
        {
            Message = "你好",
        };

        // 生成默认配置信息后退出
        await AJson.SaveAsync(info.FullName, conf);
    }
}