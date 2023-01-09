using System.Runtime.CompilerServices;
using BBTool.Config.Files;
using BBTool.Core;

namespace BBTool.Config;

public static class MessageTool
{
    public static readonly string AppName = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;

    public static readonly string AppDir = Path.GetDirectoryName(Environment.ProcessPath)!;

    public static readonly string AppDataDir = AppDir;

    public static readonly string AppLogDir = Path.Combine(AppDataDir, AppName + "_logs");

    public static readonly string
        AppHistoryDir = Path.Combine(AppDataDir, AppName + "_history");

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

            foreach (var action in ActionsAfterExit)
            {
                action.Invoke();
            }

            if (RemoveTempFilesAfterExit)
            {
                // 删除所有临时文件
                foreach (var info in Global.TempFiles)
                {
                    if (info.Exists)
                    {
                        if (info is DirectoryInfo dirInfo)
                        {
                            dirInfo.Delete(true);
                        }
                        else
                        {
                            info.Delete();
                        }
                    }
                }
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