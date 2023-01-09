using System.Text.Json;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.LowLevel;

namespace BBTool.Core.BiliApi.Video;

public class GetInfo : SimpleRequest
{
    public override string ApiPattern => "http://api.bilibili.com/x/web-interface/view?{0}id={1}";

    public async Task<VideoInfo> Send(string vid, string cookie = "")
    {
        return await GetData(obj =>
            {
                var owner = obj.GetProperty("owner");
                var staffList = new List<StaffInfo>();

                if (owner.TryGetProperty("staff", out var staff))
                {
                    // 如果存在多个成员
                    foreach (var item in staff.EnumerateArray())
                    {
                        staffList.Add(new StaffInfo
                        {
                            Title = item.GetProperty("title").GetString()!,
                            Mid = item.GetProperty("mid").GetInt64(),
                            UserName = item.GetProperty("name").GetString()!,
                        });
                    }
                }

                return new VideoInfo
                {
                    Avid = obj.GetProperty("aid").GetInt64(),
                    Mid = owner.GetProperty("mid").GetInt64(),
                    UserName = owner.GetProperty("name").GetString()!,
                    Title = obj.GetProperty("title").GetString()!,
                    Category = obj.GetProperty("tname").GetString()!,
                    PublishTime = Sys.GetDateTime(obj.GetProperty("pubdate").GetInt32()),
                    Staffs = staffList,
                };
            },
            () => HttpNew.Get(ImplementUrl(vid), cookie)
        );
    }

    protected override string ImplementUrl(params object?[] args)
    {
        if (args.Length == 0)
        {
            return "";
        }

        var vid = args.First()!.ToString()!;
        if (vid.ToLower().StartsWith("av"))
        {
            return string.Format(ApiPattern, "a", vid.Substring(2));
        }

        if (vid.ToLower().StartsWith("bv"))
        {
            return string.Format(ApiPattern, "bv", vid);
        }

        return "";
    }
}