﻿using System.Text.Json;
using BBTool.Core.Entities;
using BBTool.Core.LowLevel;

namespace BBTool.Core.Video;

public class GetRootComments : SimpleRequest
{
    public override string ApiPattern =>
        "http://api.bilibili.com/x/v2/reply?type=1&oid={0}&ps={1}&pn={2}&sort=1&nohot=1";

    public async Task<List<CommentInfo>> Send(long avid, int numPerPage, int page, string cookie = "")
    {
        return await GetData(obj =>
            {
                var comments = new List<CommentInfo>();

                int cnt = obj.GetProperty("page").GetProperty("count").GetInt32();
                if (cnt > 0)
                {
                    var replies = obj.GetProperty("replies");

                    foreach (JsonElement item in replies.EnumerateArray())
                    {
                        var info = new CommentInfo();
                        info.Id = item.GetProperty("rpid").GetInt64();
                        info.Mid = item.GetProperty("mid").GetInt64();
                        info.Message = item.GetProperty("content").GetProperty("message").GetString()!;
                        info.Count = item.GetProperty("count").GetInt32();

                        comments.Add(info);
                    }
                }

                return comments;
            },
            () => HttpNew.Get(ImplementUrl(avid, numPerPage, page), cookie)
        );
    }
}