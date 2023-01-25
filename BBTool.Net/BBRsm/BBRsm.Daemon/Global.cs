using A180.Network.Http;
using BBRsm.Core.BiliApiImpl;
using BBTool.Core.BiliApi.Entities;
using BBRsm.Core;
using A180.CoreLib.Text.Extensions;
using BBTool.Config;
using A180.CoreLib.Maths;

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

    public static Dictionary<long, UserAndCookie> Accounts => new();

    public static Dictionary<long, int> ActiveAccounts => new(); // Value：权重

    public static Dictionary<long, DateTime> BlockedAccounts => new();// Value：时间

    public static LinkedList<VideoInfo> VideoQueue => new();

    public static Dictionary<long, HashSet<long>> BlackLists => new();

    public static long CurrentAccount { get; set; } = 0; // 当前正在使用

    public static Task<int> LogoutTask { get; } = new ClearAccounts().Run(); // 是否正在执行注销

    /// <summary>
    /// 账户 - 原子操作
    /// </summary>
    public static void AddAccount(UserAndCookie user)
    {
        lock (_accountLock)
        {
            var db = RedisHelper.Database;

            // 存内存
            Accounts.Add(user.Mid, user);

            ActiveAccounts.Add(user.Mid, user.CurrentLevel);

            // 存数据库
            db.StringSet(RedisHelper.Keys.Accounts + "/" + user.Mid, user.ToJson());

            db.StringSet(RedisHelper.Keys.Active + "/" + user.Mid, user.CurrentLevel.ToString());
        }
    }

    public static void RemoveAccount(long uid)
    {
        lock (_accountLock)
        {
            var db = RedisHelper.Database;

            // 删内存
            Accounts.Remove(uid);

            // 删数据库
            db.KeyDelete(RedisHelper.Keys.Accounts + "/" + uid);

            db.KeyDelete(RedisHelper.Keys.Active + "/" + uid);

            db.KeyDelete(RedisHelper.Keys.Blocked + "/" + uid);
        }
    }

    public static List<UserAndCookie> TakeAllAccounts()
    {

        lock (_accountLock)
        {
            var db = RedisHelper.Database;

            var res = Accounts.Select(item => item.Value).ToList();

            // 删内存
            Accounts.Clear();
            ActiveAccounts.Clear();
            BlockedAccounts.Clear();

            // 删数据库
            foreach (var item in res)
            {
                db.KeyDelete(RedisHelper.Keys.Accounts + "/" + item.Mid);

                db.KeyDelete(RedisHelper.Keys.Active + "/" + item.Mid);

                db.KeyDelete(RedisHelper.Keys.Blocked + "/" + item.Mid);
            }

            return res;
        }
    }

    public static void SetAccountActive(long uid, bool active)
    {
        lock (_accountLock)
        {
            if (!Accounts.TryGetValue(uid, out var info))
            {
                return;
            }

            var db = RedisHelper.Database;
            if (active)
            {
                int level = info.CurrentLevel;

                // 操作内存
                ActiveAccounts.Add(uid, level);

                BlockedAccounts.Remove(uid);

                // 操作数据库
                db.StringSet(RedisHelper.Keys.Active + "/" + uid, level);

                db.KeyDelete(RedisHelper.Keys.Blocked + "/" + uid);
            }
            else
            {
                var now = DateTime.Now;

                // 操作内存
                ActiveAccounts.Remove(uid);

                BlockedAccounts.Add(uid, now);

                // 操作数据库
                db.KeyDelete(RedisHelper.Keys.Active + "/" + uid);

                db.StringSet(RedisHelper.Keys.Blocked + "/" + uid, now.ToJson());
            }
        }
    }

    public static UserAndCookie? SelectAccount()
    {
        lock (_accountLock)
        {
            if (Global.Accounts.Count == 0)
                return null;

            var rdList = new List<ARandom<long>.RandomConfig>();
            foreach (var item in Global.Accounts)
            {
                if (Global.ActiveAccounts.ContainsKey(item.Value.Mid))
                {
                    rdList.Add(new(item.Value.Mid, item.Value.CurrentLevel));
                }
            }

            var target = ARandom<long>.Generate(rdList);
            return Global.Accounts[target.Value];
        }
    }

    public static void RecoverData()
    {
        lock (_accountLock)
        {
            var db = RedisHelper.Database;
            var server = RedisHelper.Server;

            // 恢复账户
            var items = server.Keys(-1, $"{RedisHelper.Keys.Accounts}\\/?*");
            foreach (var item in items)
            {
                var s = db.StringGet(item);
                var account = s.ToString().FromJson<UserAndCookie>();
                Global.Accounts.Add(account.Mid, account);
            }

            // 活跃账户
            items = server.Keys(-1, $"{RedisHelper.Keys.Active}\\/?*");
            foreach (var item in items)
            {
                var key = item.ToString();
                var uid = int.Parse(key[(key.IndexOf('/') + 1)..]);

                var s = db.StringGet(item);
                var level = int.Parse(s.ToString());
                Global.ActiveAccounts.Add(uid, level);
            }

            // 睡眠账户
            items = server.Keys(-1, $"{RedisHelper.Keys.Blocked}\\/?*");
            foreach (var item in items)
            {
                var key = item.ToString();
                var uid = int.Parse(key[(key.IndexOf('/') + 1)..]);

                var s = db.StringGet(item);
                var dt = s.ToString().FromJson<DateTime>();
                Global.BlockedAccounts.Add(uid, dt);
            }
        }
    }

    /// <summary>
    /// 视频队列 - 原子操作
    /// </summary>
    public static void PushVideo(VideoInfo item)
    {
        lock (_videoLock)
        {
            VideoQueue.AddLast(item);
        }
    }

    public static VideoInfo? PopVideo()
    {

        lock (_videoLock)
        {
            if (VideoQueue.Count == 0)
            {
                return null;
            }
            var first = VideoQueue.First!.Value;
            VideoQueue.RemoveFirst();
            return first;
        }
    }

    private static object _accountLock = new();

    private static object _videoLock = new();
}
