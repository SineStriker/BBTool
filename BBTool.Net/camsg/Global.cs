namespace Camsg;

public static class Global
{
    public static readonly string AppName = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;

    public static readonly string AppDir = Path.GetDirectoryName(Environment.ProcessPath)!;

    public static readonly string AppDataDir = AppDir;

    public static readonly string AppLogDir = Path.Combine(AppDataDir, AppName + "_logs");

    public static readonly string CookiePath = Path.Combine(AppDataDir, "bbtool_cookie.txt");

    /// <summary>
    /// 全局配置
    /// </summary>
    public static AppConfig Config = new();

    public static string Cookie = "";

    public static bool RecoveryMode = false;

    public static bool DebugMode = false;
}