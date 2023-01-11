using A180.Network.Http;
using BBTool.Config;

namespace BBRsm.Daemon;

public static class Global
{
    public static readonly string RedisUrl = "127.0.0.1:6379,password=123456";

    public static int ServerPort = 14252;

    public static HttpServer? Server = null;

    public static AppConfig Config => (AppConfig)MessageTool.Config;
}