using System.Text.Json;
using BBDown.Core;
using BBTool.Config;
using BBTool.Core.Entities;
using BBTool.Core.User;
using BBTool.Core.LowLevel;
using GetInfo = BBTool.Core.Video.GetInfo;

namespace Somsg.Tasks;

public class GetVideo : BaseTask
{
    public override int TaskId => 0;

    public TaskBaseInfo Data { get; set; } = new();

    public async Task<bool> Run(string vid)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取视频信息...");

            try
            {
                Data = LoadData<TaskBaseInfo>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败，错误信息：{e.Message}");
                return false;
            }

            // 若本次未指定消息内容
            if (!string.IsNullOrEmpty(Data.SavedMessage) && string.IsNullOrEmpty(Global.Config.Message))
            {
                Logger.Log($"由于没有指定消息内容，使用日志中保存的代替");
                Global.Config.Message = Data.SavedMessage;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(vid))
            {
                Logger.LogError($"缺少视频id");
                return false;
            }

            Logger.Log("获取视频信息...");
            if (vid.ToLower().StartsWith("av") || vid.ToLower().StartsWith("bv"))
            {
                // 获取视频信息
                {
                    var api = new GetInfo();
                    var info = await api.Send(vid, MessageTool.Cookie);
                    if (info == null)
                    {
                        Logger.LogError("获取视频信息失败");
                        return false;
                    }

                    Data.VideoInfo = info;
                }

                // 获取评论信息
                {
                    var api = new GetCommentCount();
                    var info = await api.Send(Data.VideoInfo.Avid, MessageTool.Cookie);
                    if (info == null)
                    {
                        Logger.LogError("获取评论数失败");
                        return false;
                    }

                    Data.CommentInfo = info;
                }
            }
            else
            {
                Logger.LogError("非法的视频id");
                return false;
            }

            // 保存消息内容
            Data.SavedMessage = Global.Config.Message;

            // 保存日志
            SaveData(Data);
        }

        Logger.LogDebug($"AV号：{Data.VideoInfo.Avid}");

        Logger.LogColor($"作者：{Data.VideoInfo.Uploader}");
        Logger.LogColor($"发布日期：{Data.VideoInfo.PublishTime}");
        Logger.LogColor($"标题：{Data.VideoInfo.Title}");
        Logger.LogColor($"分区：{Data.VideoInfo.Category}");
        Logger.LogColor($"评论数：{Data.CommentInfo.Total}");

        return true;
    }
}