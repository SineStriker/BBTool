using System.Runtime.CompilerServices;
using StackExchange.Redis;

namespace BBRsm.Core;

public static class RedisHelper
{
    private static readonly ConfigurationOptions ConfigurationOptions = ConfigurationOptions.Parse(Rsm.RedisUrl);

    private static ConnectionMultiplexer? _redisConn = null;

    /// <summary>
    /// 单例获取
    /// </summary>
    public static ConnectionMultiplexer Connection
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            if (_redisConn == null)
            {
                // 锁定某一代码块，让同一时间只有一个线程访问该代码块
                _redisConn = ConnectionMultiplexer.Connect(ConfigurationOptions);
            }

            return _redisConn;
        }
    }

    public static IDatabase Database => Connection.GetDatabase();

    public static IServer Server
    {
        get
        {
            var conn = Connection;
            return conn.GetServer(conn.GetEndPoints().First());
        }
    }

    public static class Keys
    {
        public const string Accounts = "accounts"; // 账户

        public const string Active = "active"; // 活跃账户

        public const string Blocked = "blocked"; // 睡眠账户

        public const string Expired = "expired"; // 失效账户

        public const string SentUsers = "sent"; // 已发送的用户

        public const string Fails = "fails"; // 失败发送记录

        public const string Videos = "videos"; // 已发送的视频

        public const string BlackList = "blacklist"; // 黑名单
    }
}