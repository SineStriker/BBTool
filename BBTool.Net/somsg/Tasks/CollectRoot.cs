﻿using BBDown.Core;
using BBTool.Config;
using BBTool.Core.Entities;
using BBTool.Core.LowLevel;
using BBTool.Core.Video;

namespace Somsg.Tasks;

public class CollectRoot : BaseTask
{
    public override int TaskId => 1;

    public CommentProgress Data { get; set; } = new();

    public async Task<bool> Run(long avid, int total)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取评论区信息...");

            try
            {
                Data = LoadData<CommentProgress>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败，错误信息：{e.Message}");
                return false;
            }

            if (Data.Finished)
            {
                Logger.Log($"已获取完毕，跳过");
                return true;
            }
        }
        else
        {
            Data.Total = total;
        }

        var list = Data.Comments;
        bool failed = false;
        while (list.Count < total)
        {
            var api = new GetRootComments();
            var page = (int)((double)list.Count / AppConfig.NumPerPage) + 1;
            var comments = await api.Send(avid, AppConfig.NumPerPage, page);
            if (comments == null || comments.Count == 0)
            {
                Logger.LogError($"获取失败：{api.ErrorMessage}");
                failed = true;
                break;
            }

            list.AddRange(comments);

            var first = comments.First();
            Logger.Log(
                $"{list.Count}/{total} 已获取{comments.Count}条评论，第一条为\"{first.UserName}\"发送的：{Text.ElideString(first.Message.Replace("\n", " "), 10)}");

            // 避免发送请求太快，设置延时
            if (!Sleep(Global.Config.GetTimeout))
            {
                failed = true;
                break;
            }

            if (comments.Count < AppConfig.NumPerPage)
            {
                Data.Finished = true;
                break;
            }
        }

        // 保存日志
        SaveData(Data);

        return !failed;
    }
}