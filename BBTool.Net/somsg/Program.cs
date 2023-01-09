// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text.Json;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Commands.Extensions;
using BBTool.Config.Files;
using BBTool.Core.LowLevel;
using Somsg.Commands;

namespace Somsg;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // 添加终端编码信息
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        // 设置默认配置信息
        MessageTool.Config = new AppConfig();

        // 介绍信息
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
        var intro = $"Search and Message Tool of Bilibili, version {ver.Major}.{ver.Minor}.{ver.Build}.";

        Console.WriteLine(intro);
        Console.WriteLine();

        var rootCommand = new AppCommand();
        rootCommand.SetRoutine(WorkRoutine);

        var parserBuilder = new CommandLineBuilder(rootCommand);
        parserBuilder.AddCookiePath(); // 添加 Cookie 选项
        parserBuilder.AddGlobal().AddPost();

        // 开始解析
        var parser = parserBuilder.Build();
        var code = await parser.InvokeAsync(args);

        // 同意主程序域退出
        MessageTool.AcceptExit = 1;

        return code;
    }

    static async Task WorkRoutine(InvocationContext context)
    {
        // 获取用户信息
        var task0 = new CheckUserTask();
        if (!await task0.Run())
        {
            return;
        }

        var user = task0.Data;
        Console.WriteLine();

        // 删除旧的日志
        if (!MessageTool.RecoveryMode)
        {
            Sys.RemoveDirRecursively(MessageTool.AppLogDir);
        }

        Directory.CreateDirectory(MessageTool.AppLogDir);
        Directory.CreateDirectory(MessageTool.AppHistoryDir);

        // 全局捕获关闭事件
        MessageTool.InstallInterruptFilter();

        // 开始执行任务
        Logger.Log("第一步");

        // 保存任务历史
        {
            // var history = new History
            // {
            //     Avid = videoInfo.Avid,
            //     Users = users.Select(item => item.Mid).ToList(),
            //     ErrorAttempts = task4.SendProgress.ErrorAttempts,
            // };
            //
            // var filename = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".json";
            // var path = Path.Combine(MessageTool.AppHistoryDir, filename);
            // File.WriteAllText(path, JsonSerializer.Serialize(history, Sys.UnicodeJsonSerializeOption()));
            // Logger.Log($"保存本次任务信息到\"{filename}\"中");
        }
    }
}