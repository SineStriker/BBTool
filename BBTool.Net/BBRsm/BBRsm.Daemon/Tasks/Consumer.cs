using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBRsm.Core;
using BBRsm.Core.BiliApiImpl;
using BBTool.Config;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Codes;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.User;

namespace BBRsm.Daemon.Tasks;

/// <summary>
/// 消息类
/// </summary>
public class Consumer : BaseTask
{
    public Consumer(int tid = -1) : base(tid)
    {
    }

    public async Task<int> Run()
    {
        int ret = 0;
        int count = 0;

        var db = RedisHelper.Database;

        // 大循环
        while (true)
        {
            bool httpSent = false;
            bool netError = false;
            bool tooFrequent = false;
            bool expired = false;
            bool skip = false;
            UserAndCookie account;

            using (var guard = new LocalTaskGuard())
            {
                string key;

                // 如果私信为空，等待
                while (string.IsNullOrEmpty(Global.Config.Message))
                {
                    if (!guard.Sleep(Global.Config.WaitTimeout))
                    {
                        ret = -2;
                        goto exit;
                    }
                }

                // 如果活跃账户为空，等待
                while (Global.Accounts.Count == 0)
                {
                    if (!guard.Sleep(Global.Config.WaitTimeout))
                    {
                        ret = -2;
                        goto exit;
                    }
                }

                // 选择合适的发送者
                //...
                account = new();

                var sender = account.Mid;
                var cookie = account.Cookie;

                // 更新睡眠队列
                {
                    var keys = new List<long>();
                    foreach (var item in Global.BlockedAccounts)
                    {
                        if (DateTime.Now > item.Value.AddMicroseconds(Global.Config.BlockTimeout))
                        {
                            keys.Add(item.Key);
                        }
                    }

                    foreach (var item in keys)
                    {
                        var info = Global.Accounts[item];
                        Global.BlockedAccounts.Remove(item);
                        Global.ActiveAccounts.Add(item, info.CurrentLevel);
                    }
                }

                // 如果资源队列为空，等待
                while (Global.VideoQueue.Count == 0)
                {
                    if (!guard.Sleep(Global.Config.WaitTimeout))
                    {
                        ret = -2;
                        goto exit;
                    }
                }

                // 取出第一个视频
                var videoInfo = Global.VideoQueue.First!.Value;
                Global.VideoQueue.RemoveFirst();

                var mid = videoInfo.Mid;
                var uname = videoInfo.UserName;
                var unameFormatted = AStrings.Pad(uname, 20);

                var logPrefix = $"{count + 1} {unameFormatted}";

                // 查找数据库
                while (true)
                {
                    // 判断是否在黑名单中
                    if (!Global.BlackLists.TryGetValue(sender, out var hs))
                    {
                        hs = new HashSet<long>();
                        Global.BlackLists.Add(sender, hs);
                    }
                    else if (hs.Contains(mid))
                    {
                        Logger.LogWarn($"{logPrefix} 对方已将您加入黑名单，跳过");
                        skip = true;
                        break;
                    }

                    // 判断是否已发送
                    key = RedisHelper.Keys.SentUsers + "/" + mid;
                    if (db.KeyExists(key))
                    {
                        Logger.LogWarn($"{logPrefix} 已成功发送过，跳过");
                        skip = true;
                        break;
                    }

                    // 判断上次失败原因
                    key = RedisHelper.Keys.Fails + "/" + mid;
                    var val = db.StringGet(key);
                    if (!val.IsNullOrEmpty)
                    {
                        var attempt = val.ToString().FromJson<FailAttempt>();
                        switch (attempt.Code)
                        {
                            case (int)MessageReturn.ErrorCode.StrangerLimit:
                            case (int)MessageReturn.ErrorCode.BlackList:
                                Logger.LogWarn($"{logPrefix} {attempt.Message}，跳过");
                                skip = true;
                                break;
                        }
                    }

                    break;
                }

                // 获取最近消息
                if (!skip)
                {
                    var api = new GetRecentTalk();
                    var msgList = await api.Send(mid, 20, cookie);

                    if (api.Code != 0)
                    {
                        Logger.LogWarn($"{logPrefix} {api.ErrorMessage}");
                        skip = true;

                        // 失败尝试存入数据库
                        key = RedisHelper.Keys.Fails + "/" + mid;
                        var attempt = new FailAttempt
                        {
                            Code = api.Code,
                            Message = api.ErrorMessage,
                            UserName = uname,
                        };

                        db.StringSet(key, attempt.ToJson());
                    }
                    else if (msgList != null && msgList.Count > 0)
                    {
                        Logger.LogWarn($"{logPrefix} 存在最近消息，跳过");
                        skip = true;
                    }

                    httpSent = true;
                }

                // 发送消息
                if (!skip)
                {
                    var api = new SendMessage();
                    await api.Send(sender, mid, Global.Config.Message, cookie);

                    var code = api.Code;
                    if (code != 0)
                    {
                        if (code < 0)
                        {
                            // 必须中断的错误
                            Logger.LogError($"{logPrefix} 致命错误：{api.ErrorMessage}");
                            netError = true;
                        }
                        else if (code == (int)MessageReturn.ErrorCode.TooFrequent)
                        {
                            // 频率过高
                            Logger.LogError(api.ErrorMessage);
                            tooFrequent = true;
                        }
                        else
                        {
                            // 记录遇到错误的用户
                            Logger.LogWarn($"{logPrefix} 发送消息跳过：{api.ErrorMessage}");
                        }

                        // 失败尝试存入数据库
                        key = RedisHelper.Keys.Fails + "/" + mid;
                        var attempt = new FailAttempt
                        {
                            Code = api.Code,
                            Message = api.ErrorMessage,
                            UserName = uname,
                        };

                        db.StringSet(key, attempt.ToJson());
                    }
                    else
                    {
                        Logger.LogColor($"{logPrefix} 发送消息成功");
                        count++;

                        // 成功记录存入数据库
                        key = RedisHelper.Keys.SentUsers + "/" + mid;
                        db.StringSet(key, "1");
                    }

                    httpSent = true;
                }

                // 避免发送请求太快，设置延时
                if (httpSent && !guard.Sleep(skip ? MessageTool.Config.GetTimeout : MessageTool.Config.MessageTimeout))
                {
                    ret = -2;
                    goto exit;
                }
            }

            // 账号失效
            if (expired)
            {
            }

            // 发送触发高频上限
            if (tooFrequent)
            {
                Global.BlockedAccounts.Add(account.Mid, DateTime.Now);
                Global.ActiveAccounts.Remove(account.Mid);
            }

            // 网络错误，等待网络恢复
            if (netError)
            {
                var pingTask = new PingTask();
                ret = await pingTask.Run();
                if (ret != 0)
                {
                    goto exit;
                }
            }
            
            // 下一轮循环
        }

        exit:

        Logger.LogColor($"总共发送给了{count}个用户");

        return ret;
    }
}