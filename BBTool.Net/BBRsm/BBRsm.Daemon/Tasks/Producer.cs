using System.Net.NetworkInformation;
using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBRsm.Core;
using BBTool.Config;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Search;

namespace BBRsm.Daemon.Tasks;

/// <summary>
/// 检索类
/// </summary>
public class Producer : BaseTask
{
    public Producer(int tid = -1) : base(tid)
    {
    }

    public async Task<int> Run()
    {
        int ret = 0;
        var db = RedisHelper.Database;

        // 大循环
        while (true)
        {
            bool finishedOnce = false;
            bool netError = false;

            string key;
            bool flag;

            using (var guard = new LocalTaskGuard())
            {
                // 如果关键词为空，等待
                flag = true;
                while (string.IsNullOrEmpty(Global.Config.KeyWord))
                {
                    if (flag)
                    {
                        Logger.Log("等待指定关键词...");
                        flag = false;
                    }

                    if (!guard.Sleep(Global.Config.WaitTimeout))
                    {
                        ret = -2;
                        break;
                    }
                }

                // 检索
                int page = 0;
                for (;; page++)
                {
                    var api = new SearchVideo();
                    var res = await api.Send(
                        Global.Config.KeyWord,
                        "pubdate",
                        Global.Config.PartitionNum,
                        page + 1,
                        MessageTool.Cookie
                    );

                    if (api.Code != 0 || res == null)
                    {
                        Logger.LogError($"获取失败：{api.ErrorMessage}");
                        netError = true;
                        break;
                    }

                    // 获取到的总数为 0 或没有获取到任何信息，则结束
                    if (res.NumPages == 0 || res.Videos.Count == 0)
                    {
                        Logger.LogWarn($"当前页数超过上限，结束");
                        finishedOnce = true;
                        break;
                    }

                    // 判断是否遇到过这个视频
                    foreach (var item in res.Videos)
                    {
                        key = RedisHelper.Keys.Videos + "/" + item.Avid;
                        if (db.KeyExists(key))
                        {
                            // 已存在视频
                            Logger.LogWarn($"遇到重复视频：av{item.Avid}");
                            finishedOnce = true;
                            break;
                        }

                        // 存内存
                        Global.VideoQueue.AddLast(item);

                        // 存数据库
                        db.StringSet(key, item.ToJson());
                    }

                    var first = res.Videos.First();
                    Logger.Log(
                        $"{page + 1}/{res.NumPages} 已获取{res.Videos.Count}条视频信息，第一条为\"{first.UserName}\"的：{first.Title.Replace("\n", " ").Elide(10)}，发布日期{first.PublishTime.ToString("yyyy-MM-dd HH:mm:ss")}");

                    // 避免发送请求太快，设置延时
                    if (!guard.Sleep(Global.Config.GetTimeout))
                    {
                        ret = -2;
                        goto exit;
                    }
                }
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

            // 搜索间隔
            if (finishedOnce)
            {
                using (var guard = new LocalTaskGuard())
                {
                    // 设置等待间隔
                    if (!guard.Sleep(Global.Config.MessageTimeout))
                    {
                        ret = -2;
                        goto exit;
                    }
                }
            }

            // 进入下一次循环
        }

        exit:

        return ret;
    }
}