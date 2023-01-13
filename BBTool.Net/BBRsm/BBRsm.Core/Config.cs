namespace BBRsm.Core;

public static class Rsm
{
    public const string AppDesc = "定期获取B站指定分区最新视频，给UP主批量发送私信";
    
    /// <summary>
    /// Redis 配置
    /// </summary>
    public static readonly string RedisUrl = "127.0.0.1:6379,password=123456";
    
    /// <summary>
    /// 服务端口号
    /// </summary>
    public static int ServerPort = 14252;

    public static string ServerUrl => $"http://localhost:{ServerPort}";
}