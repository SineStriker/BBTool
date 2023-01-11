using System.Text.Json;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Tasks;
using BBTool.Core.BiliApi.Video;
using GetUserInfo = BBTool.Core.BiliApi.User.GetInfo;
using GetVideoInfo = BBTool.Core.BiliApi.Video.GetInfo;

namespace Camsg.Tasks;

public class GetVideo : BaseTask
{
    public TaskBaseInfo Data { get; set; } = new();

    public GetVideo(int tid) : base(tid)
    {
    }

    public async Task<int> Run(Action beforeSave = null)
    {
        string vid = Global.VideoId;

        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取视频信息...");

            try
            {
                Data = await LoadDataAsync<TaskBaseInfo>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败：{e.Message}");
                return -1;
            }

            // 若本次未指定消息内容
            if (string.IsNullOrEmpty(MessageTool.Config.Message))
            {
                if (!string.IsNullOrEmpty(Data.SavedMessage))
                {
                    Logger.Log($"由于没有指定消息内容，使用日志中保存的代替");
                    MessageTool.Config.Message = Data.SavedMessage;
                }
                else
                {
                    Logger.LogWarn("缺少消息内容");
                    return -1;
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(vid))
            {
                Logger.LogError($"缺少视频id");
                return -1;
            }

            Logger.Log("获取视频信息...");
            if (vid.ToLower().StartsWith("av") || vid.ToLower().StartsWith("bv"))
            {
                // 获取视频信息
                {
                    var api = new GetVideoInfo();
                    var info = await api.Send(vid, MessageTool.Cookie);
                    if (info == null)
                    {
                        Logger.LogError($"获取视频信息失败：{api.ErrorMessage}");
                        return api.Code;
                    }

                    Logger.LogDebug($"AV号：{info.Avid}");
                    Logger.LogColor($"作者：{info.UserName}");
                    Logger.LogColor($"发布日期：{info.PublishTime}");
                    Logger.LogColor($"标题：{info.Title}");
                    Logger.LogColor($"分区：{info.Category}");

                    Data.VideoInfo = info;
                }

                // 获取评论信息
                {
                    var api = new GetCommentCount();
                    var info = await api.Send(Data.VideoInfo.Avid, MessageTool.Cookie);
                    if (info == null)
                    {
                        Logger.LogError($"获取评论数失败：{api.ErrorMessage}");
                        return api.Code;
                    }

                    Logger.LogColor($"评论数：{info.Total}");

                    Data.CommentInfo = info;
                }
            }
            else
            {
                Logger.LogError("非法的视频id");
                return -1;
            }

            // 检查是否有消息内容
            if (string.IsNullOrEmpty(Global.Config.Message))
            {
                Logger.LogWarn("缺少消息内容");
                return -1;
            }

            // 保存消息内容
            Data.SavedMessage = Global.Config.Message;
            
            if (beforeSave != null)
            {
                beforeSave.Invoke();
            }

            // 保存日志
            await SaveDataAsync(Data);
        }

        return 0;
    }
}