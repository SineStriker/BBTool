using BBTool.Config;

namespace Somsg;

public static class Global
{
    public static string KeyWord = "";

    public static AppConfig Config => (AppConfig)MessageTool.Config;
}