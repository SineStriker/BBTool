using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Search;
using BBTool.Core.LowLevel;

namespace Somsg.Tasks;

public class SearchTask : BaseTask
{
    public SearchProgress Data { get; set; } = new();

    public SearchTask(int tid) : base(tid)
    {
    }

    public async Task<bool> Run()
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取视频信息...");

            try
            {
                Data = await LoadDataAsync<SearchProgress>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败：{e.Message}");
                return false;
            }

            if (Data.Finished)
            {
                Logger.Log($"已获取完毕，跳过");
                return true;
            }
        }

        var keyword = Global.KeyWord;
        var order = (AppConfig.SortOrder)Global.Config.SortOrderNum;

        bool failed = false;
        using (var guard = new LocalTaskGuard())
        {
            int page = Data.CurrentPage;
            var list = Data.Videos;
            for (;; page++)
            {
                var api = new SearchVideo();
                var res = await api.Send(
                    keyword,
                    order.ToString().ToLower(),
                    Global.Config.PartitionNum,
                    page + 1,
                    MessageTool.Cookie
                );

                if (api.Code != 0 || res == null)
                {
                    Logger.LogError($"获取失败：{api.ErrorMessage}");
                    failed = true;
                    break;
                }

                // 获取到的总数为 0 或没有获取到任何信息，则结束
                if (res.NumPages == 0 || res.Videos.Count == 0)
                {
                    Logger.LogError($"当前页数超过上限，结束");
                    Data.Finished = true;
                    break;
                }

                list.AddRange(res.Videos);

                // 立即更新进度值
                Data.CurrentPage = page + 1;
                Data.PagesExpected = res.NumPages;
                Data.TotalExpected = res.NumResults;

                var first = res.Videos.First();
                Logger.Log(
                    $"{page + 1}/{res.NumPages} 已获取{res.Videos.Count}条视频信息，第一条为\"{first.UserName}\"的：{Text.ElideString(first.Title.Replace("\n", " "), 10)}，发布日期{first.PublishTime.ToString("yyyy-MM-dd HH:mm:ss")}");

                // 避免发送请求太快，设置延时
                if (!guard.Sleep(Global.Config.GetTimeout))
                {
                    failed = true;
                    break;
                }
            }
        }

        await SaveDataAsync(Data);

        return !failed;
    }
}