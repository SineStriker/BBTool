using System.Text.Json;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.Network;

namespace BBTool.Core.BiliApi.Video;

public class GetSubComments : SimpleRequest
{
    public override string ApiPattern =>
        "http://api.bilibili.com/x/v2/reply/reply?type=1&oid={0}&root={1}&ps={2}&pn={3}";

    public async Task<List<CommentInfo>?> Send(long avid, long replyId, int numPerPage, int page, string cookie = "")
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
                        info.UserName = item.GetProperty("member").GetProperty("uname").GetString()!;
                        info.Message = item.GetProperty("content").GetProperty("message").GetString()!;
                        info.Count = item.GetProperty("count").GetInt32();

                        comments.Add(info);
                    }
                }

                return comments;
            },
            () => HttpWrapper.Get(ImplementUrl(avid, replyId, numPerPage, page), cookie)
        );
    }
}