using System.Collections;
using System.Runtime.CompilerServices;
using A180.CoreLib.Collections.Extensions;
using A180.CoreLib.Kernel.Extensions;
using BBTool.Config.Files;
using BBTool.Core;

namespace BBTool.Config;

public static class MessageTool
{
    public static readonly string AppName = Environment.ProcessPath!.BaseName();

    public static readonly string AppDir = Environment.ProcessPath!.DirName()!;

    public static readonly string AppDataDir = AppDir;

    public static readonly string AppLogDir = Path.Combine(AppDataDir, AppName + "_logs");

    public static readonly string
        AppHistoryDir = Path.Combine(AppDataDir, AppName + "_history");

    public static readonly string HistoryFileFormat = "yyyy-MM-dd_HH-mm-ss";

    public static string CookiePath = Path.Combine(AppDataDir, "bbtool_cookie.txt");

    public static string Cookie = "";

    /// <summary>
    /// 全局配置
    /// </summary>
    public static MessageConfig Config = new();

    /// <summary>
    /// 信号量
    /// </summary>
    public static volatile int Interrupt = 0;

    public static volatile int AcceptExit = 0;

    public static bool HasInterruptFilter { get; internal set; } = false;

    public static bool RemoveTempFilesAfterExit = true;

    public static List<Action> ActionsAfterExit = new();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void InstallInterruptFilter()
    {
        // 必须只能单一线程访问
        if (HasInterruptFilter)
        {
            return;
        }

        HasInterruptFilter = true;

        // 添加退出事件
        AppDomain appd = AppDomain.CurrentDomain;
        appd.ProcessExit += (s, e) =>
        {
            if (Interrupt == 0)
            {
                Interrupt = 1;
            }

            while (AcceptExit == 0)
            {
                // 等待主线程同意退出
            }

            ActionsAfterExit.ForEach(item => item.Invoke());

            if (RemoveTempFilesAfterExit)
            {
                // 删除所有临时文件
                Global.TempFiles.ForEach(item => item.Remove());
            }
        };

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // true: 不导致退出；false: 会导致退出

            Interrupt = 1;

            // Console.WriteLine("Ctrl+C 中断被触发");
        };
    }

    public static bool RecoveryMode = false;

    public static bool DebugMode = false;
}