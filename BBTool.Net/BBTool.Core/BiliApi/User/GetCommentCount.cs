using System.Text.Json;
using BBTool.Core.Entities;
using BBTool.Core.LowLevel;

namespace BBTool.Core.User;

public class GetCommentCount : SimpleRequest
{
    public override string ApiPattern => "http://api.bilibili.com/x/v2/reply?type=1&oid={0}&ps=1";

    public string ImplementUrl(long avid)
    {
        return string.Format(ApiPattern, avid);
    }

    public async Task<CommentCount> Send(long avid, string cookie = "")
    {
        return await GetData(obj =>
                new CommentCount
                {
                    Root = obj.GetProperty("page").GetProperty("count").GetInt32(),
                    Total = obj.GetProperty("page").GetProperty("acount").GetInt32(),
                },
            () => HttpNew.Get(ImplementUrl(avid), cookie)
        );
    }
}