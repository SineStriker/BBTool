using System.Text;
using System.Text.Json;
using BBDown.Core;
using BBDown.Core.Util;

namespace CmtMsg;

public class Video
{
    public static string DEBUG_PATH = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "CmtMsg.debug.txt");

    public class VideoInfo
    {
        public long Avid { get; set; } = -1;
        public string Uploader { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime PublishTime { get; set; }
    }

    public class CommentInfo
    {
        public long Id { get; set; } = -1;
        public long UserId { get; set; }
        public long Count { get; set; }
    }

    public static VideoInfo GetVideoInfo(string id)
    {
        var info = new VideoInfo();
        try
        {
            string api = $"http://api.bilibili.com/x/web-interface/view?{id.ToLower().Substring(0, 2)}id={id}";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            info.Avid = dataObj.GetProperty("aid").GetInt64();
            info.Uploader = dataObj.GetProperty("owner").GetProperty("name").GetString();
            info.Title = dataObj.GetProperty("title").GetString();
            info.Category = dataObj.GetProperty("tname").GetString();
            info.PublishTime = GetDateTime(dataObj.GetProperty("pubdate").GetInt32());
        }
        catch (Exception e)
        {
        }

        return info;
    }

    public static int GetCommentCount(long avid)
    {
        int cnt = -1;
        try
        {
            string api = $"http://api.bilibili.com/x/v2/reply/count?type=1&oid={avid}";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            cnt = dataObj.GetProperty("count").GetInt32();
        }
        catch (Exception e)
        {
        }

        return cnt;
    }


    public static int GetRootCommentCount(long avid)
    {
        int cnt = -1;
        try
        {
            string api = $"http://api.bilibili.com/x/v2/reply?type=1&oid={avid}&ps=1";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            cnt = dataObj.GetProperty("page").GetProperty("count").GetInt32();
        }
        catch (Exception e)
        {
        }

        return cnt;
    }

    public static List<CommentInfo> GetRootComments(long avid, int numPerPage, int page)
    {
        var comments = new List<CommentInfo>();

        try
        {
            string api =
                $"http://api.bilibili.com/x/v2/reply?type=1&oid={avid}&ps={numPerPage}&pn={page}&sort=1&nohot=1";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            int cnt = dataObj.GetProperty("page").GetProperty("count").GetInt32();
            if (cnt > 0)
            {
                var replies = dataObj.GetProperty("replies");
                
                var fs = new FileStream(DEBUG_PATH, FileMode.Append);
                var stream = new StreamWriter(fs);
                stream.WriteLine(api);

                foreach (JsonElement item in replies.EnumerateArray())
                {
                    var info = new CommentInfo();
                    info.Id = item.GetProperty("rpid").GetInt64();
                    info.UserId = item.GetProperty("mid").GetInt64();
                    info.Count = item.GetProperty("count").GetInt64();

                    {
                        stream.WriteLine(
                            item.GetProperty("member").GetProperty("uname").GetString() + " : " +
                            item.GetProperty("content").GetProperty("message").GetString(), Encoding.UTF8);
                    }

                    comments.Add(info);
                }

                stream.WriteLine();
                stream.Close();
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            comments = null;
        }

        return comments;
    }

    private static DateTime GetDateTime(int timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
}