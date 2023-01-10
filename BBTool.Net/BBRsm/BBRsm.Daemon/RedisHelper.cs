using BBTool.Core;
using StackExchange.Redis;

namespace BBRsm.Daemon;

public static class RedisHelper
{
    private static readonly ConfigurationOptions ConfigurationOptions = ConfigurationOptions.Parse(Global.RedisUrl);

    private static readonly object Locker = new();
    private static ConnectionMultiplexer _redisConn;

    /// <summary>
    /// 单例获取
    /// </summary>
    public static ConnectionMultiplexer Coonection
    {
        get
        {
            if (_redisConn == null)
            {
                // 锁定某一代码块，让同一时间只有一个线程访问该代码块
                lock (Locker)
                {
                    if (_redisConn == null || !_redisConn.IsConnected)
                    {
                        _redisConn = ConnectionMultiplexer.Connect(ConfigurationOptions);
                    }
                }
            }

            return _redisConn;
        }
    }
}