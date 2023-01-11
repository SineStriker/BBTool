using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Files;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Video;

namespace Camsg.Tasks;

public class CollectRoot : BaseTask
{
    public CommentProgress Data { get; set; } = new();

    public CollectRoot(int tid) : base(tid)
    {
    }

    public async Task<int> Run(long avid, int total)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取评论区信息...");

            try
            {
                Data = await LoadDataAsync<CommentProgress>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败，错误信息：{e.Message}");
                return -1;
            }

            if (Data.Finished)
            {
                Logger.Log($"已获取完毕，跳过");
                return 0;
            }
        }
        else
        {
            Data.Total = total;
        }

        int ret = 0;
        using (var guard = new LocalTaskGuard())
        {
            var list = Data.Comments;
            while (list.Count < total)
            {
                var api = new GetRootComments();
                var page = (int)((double)list.Count / MessageConfig.NumPerPage) + 1;
                var comments = await api.Send(avid, MessageConfig.NumPerPage, page);
                if (comments == null || api.Code != 0)
                {
                    Logger.LogError($"获取失败：{api.ErrorMessage}");
                    ret = api.Code;
                    break;
                }

                if (comments.Count > 0)
                {
                    list.AddRange(comments);

                    var first = comments.First();
                    Logger.Log(
                        $"{list.Count}/{total} 已获取{comments.Count}条评论，第一条为\"{first.UserName}\"发送的：{first.Message.Replace("\n", " ").Elide(10)}");

                    // 避免发送请求太快，设置延时
                    if (!guard.Sleep(Global.Config.GetTimeout))
                    {
                        ret = -2;
                        break;
                    }
                }

                if (comments.Count < MessageConfig.NumPerPage)
                {
                    Data.Finished = true;
                    break;
                }
            }
        }

        // 保存日志
        await SaveDataAsync(Data);

        return ret;
    }
}