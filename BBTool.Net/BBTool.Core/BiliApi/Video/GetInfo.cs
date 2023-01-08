using System.Text.Json;
using BBTool.Core.Entities;
using BBTool.Core.LowLevel;

namespace BBTool.Core.Video;

public class GetInfo : SimpleRequest
{
    public override string ApiPattern => "http://api.bilibili.com/x/web-interface/view?{0}id={1}";

    public VideoInfo Send(string vid, string cookie = "")
    {
        return GetData(obj =>
                new VideoInfo
                {
                    Avid = obj.GetProperty("aid").GetInt64(),
                    Uploader = obj.GetProperty("owner").GetProperty("name").GetString(),
                    Title = obj.GetProperty("title").GetString(),
                    Category = obj.GetProperty("tname").GetString(),
                    PublishTime = Sys.GetDateTime(obj.GetProperty("pubdate").GetInt32()),
                },
            () => Http.Get(ImplementUrl(vid.Substring(0, 2).ToLower(), vid), cookie)
        );
    }
}