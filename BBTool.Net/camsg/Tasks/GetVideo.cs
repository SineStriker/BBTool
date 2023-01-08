using System.Text.Json;
using BBDown.Core;
using BBTool.Core.Entities;
using BBTool.Core.User;
using BBTool.Core.LowLevel;
using GetInfo = BBTool.Core.Video.GetInfo;

namespace Camsg.Tasks;

public class GetVideo : BaseTask
{
    public override int TaskId => 0;

    public override string TaskLogDir => Global.AppLogDir;

    public class TaskData
    {
        public VideoInfo VideoInfo { get; set; }

        public CommentCount CommentInfo { get; set; }
    }

    public TaskData Data { get; set; } = new();

    public async Task<bool> Run(string vid)
    {
        if (Global.RecoveryMode)
        {
            Logger.Log("从日志中获取视频信息...");

            try
            {
                Data = JsonSerializer.Deserialize<TaskData>(File.ReadAllText(DataPath),
                    Sys.UnicodeJsonSerializeOption())!;
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败，错误信息：{e.Message}");
                return false;
            }
        }
        else
        {
            Logger.Log("获取视频信息...");
            if (vid.ToLower().StartsWith("av") || vid.ToLower().StartsWith("bv"))
            {
                // 获取视频信息
                {
                    var api = new GetInfo();
                    var info = await api.Send(vid, Global.Cookie);
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
                    var info = await api.Send(Data.VideoInfo.Avid, Global.Cookie);
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

            // 保存任务日志
            File.WriteAllBytes(DataPath, JsonSerializer.SerializeToUtf8Bytes(Data, Sys.UnicodeJsonSerializeOption()));
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