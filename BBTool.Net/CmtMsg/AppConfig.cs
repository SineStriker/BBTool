namespace CmtMsg;

public class AppConfig
{
    public readonly static string APP_NAME = Path.GetFileNameWithoutExtension(Environment.ProcessPath)!;

    public readonly static string APP_DIR = Path.GetDirectoryName(Environment.ProcessPath)!;

    public readonly static string APP_LOG_DIR = Path.Combine(APP_DIR, APP_NAME + "_logs");

    public readonly static string COOKIE_PATH = Path.Combine(APP_DIR, "BBDown.data");

    public static int DEFAULT_GET_INTERVAL = 1000;

    public static int DEFAULT_MESSAGE_INTERVAL = 5000;
}