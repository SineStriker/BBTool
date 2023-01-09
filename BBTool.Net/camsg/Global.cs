namespace Camsg;

public static class Global
{
    public static readonly string AppName = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;

    public static readonly string AppDir = Path.GetDirectoryName(Environment.ProcessPath)!;

    public static readonly string AppDataDir = AppDir;

    public static readonly string AppLogDir = Path.Combine(AppDataDir, AppName + "_logs");

    public static readonly string AppHistoryDir = Path.Combine(AppDataDir, AppName + "_history");

    public static readonly string CookiePath = Path.Combine(AppDataDir, "bbtool_cookie.txt");

    /// <summary>
    /// 信号量
    /// </summary>
    public static volatile int Interrupt = 0;

    public static volatile int AcceptExit = 0;

    public static void InstallInterruptFilter()
    {
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
        };
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // true: 不导致退出；false: 会导致退出

            Interrupt = 1;

            // Console.WriteLine("Ctrl+C 中断被触发");
        };
    }

    /// <summary>
    /// 全局配置
    /// </summary>
    public static AppConfig Config = new();

    public static string Cookie = "";

    public static bool RecoveryMode = false;

    public static bool DebugMode = false;
}