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

    public CommentCount Send(long avid, string cookie = "")
    {
        return GetData(obj =>
                new CommentCount
                {
                    Root = obj.GetProperty("page").GetProperty("count").GetInt32(),
                    Total = obj.GetProperty("page").GetProperty("acount").GetInt32(),
                },
            () => Http.Get(ImplementUrl(avid), cookie)
        );
    }
}