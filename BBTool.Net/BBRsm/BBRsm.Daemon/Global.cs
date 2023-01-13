using A180.Network.Http;
using BBRsm.Core.BiliApiImpl;
using BBTool.Config;

namespace BBRsm.Daemon;

public static class Global
{
    /// <summary>
    /// 服务端配置
    /// </summary>
    public static HttpServer? Server;

    /// <summary>
    /// 全局配置
    /// </summary>
    public static AppConfig Config => (AppConfig)MessageTool.Config;

    public static Dictionary<long, UserAndCookie> Accounts = new();

    public static HashSet<long> SentUsers = new();
}