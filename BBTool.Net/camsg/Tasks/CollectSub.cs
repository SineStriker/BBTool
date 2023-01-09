﻿using BBDown.Core;
using BBTool.Config;
using BBTool.Core.Entities;
using BBTool.Core.LowLevel;
using BBTool.Core.Video;

namespace Camsg.Tasks;

public class CollectSub : BaseTask
{
    public override int TaskId => 2;

    public SubCommentProgress Data { get; set; } = new();

    public async Task<bool> Run(long avid, List<CommentInfo> rootComments)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取评论区信息...");

            try
            {
                Data = LoadData<SubCommentProgress>();
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

        bool failed = false;
        int idx = 0;
        int sum = rootComments.Count;
        foreach (var item in rootComments)
        {
            idx++;

            // 如果没有评论
            if (item.Count == 0)
            {
                continue;
            }

            // 如果有评论
            var total = item.Count;

            CommentProgress commentList;
            if (!Data.SubComments.TryGetValue(item.Id, out commentList))
            {
                commentList = new CommentProgress();
                Data.SubComments.Add(item.Id, commentList);
            }

            // 如果已经获取完了
            if (commentList.Finished)
            {
                continue;
            }

            var list = commentList.Comments;
            while (list.Count < total)
            {
                var api = new GetSubComments();
                var page = (int)((double)list.Count / AppConfig.NumPerPage) + 1;
                var comments = await api.Send(avid, item.Id, AppConfig.NumPerPage, page);
                if (comments == null || comments.Count == 0)
                {
                    Logger.LogError($"获取失败：{api.ErrorMessage}");
                    failed = true;
                    break;
                }

                list.AddRange(comments);

                var first = comments.First();
                Logger.Log(
                    $"{idx}/{sum} {list.Count}/{total} 已获取{comments.Count}条评论，第一条为\"{first.UserName}\"发送的：{Text.ElideString(first.Message.Replace("\n", " "), 10)}"
                );

                // 避免发送请求太快，设置延时
                if (!Sleep(Global.Config.GetTimeout))
                {
                    failed = true;
                    break;
                }

                if (comments.Count < AppConfig.NumPerPage)
                {
                    commentList.Finished = true;
                    break;
                }
            }

            if (failed)
            {
                break;
            }
        }

        if (!failed)
        {
            Data.Finished = true;
        }

        // 保存日志
        SaveData(Data);

        return !failed;
    }
}