#define NO_DEBUG_FILE

using System.Text;
using System.Text.Json;
using BBDown.Core;
using BBDown.Core.Util;

namespace CmtMsg;

public class Video
{
#if ! NO_DEBUG_FILE
    public static string DEBUG_PATH = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "CmtMsg.debug.txt");
#endif

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
        public int Count { get; set; }
    }

    public class CommentCountInfo
    {
        public int Root { get; set; } = -1;
        public int Total { get; set; } = -1;
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
                Logger.LogDebug($"错误码：{code}；错误信息：{json.GetProperty("message").GetString()}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            info.Avid = dataObj.GetProperty("aid").GetInt64();
            info.Uploader = dataObj.GetProperty("owner").GetProperty("name").GetString();
            info.Title = dataObj.GetProperty("title").GetString();
            info.Category = dataObj.GetProperty("tname").GetString();
            info.PublishTime = Sys.GetDateTime(dataObj.GetProperty("pubdate").GetInt32());
        }
        catch (Exception e)
        {
        }

        return info;
    }

    public static CommentCountInfo GetCommentCount(long avid)
    {
        var info = new CommentCountInfo();
        try
        {
            string api = $"http://api.bilibili.com/x/v2/reply?type=1&oid={avid}&ps=1";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}；错误信息：{json.GetProperty("message").GetString()}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            info.Root = dataObj.GetProperty("page").GetProperty("count").GetInt32();
            info.Total = dataObj.GetProperty("page").GetProperty("acount").GetInt32();
        }
        catch (Exception e)
        {
            info = null;
        }

        return info;
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
                Logger.LogDebug($"错误码：{code}；错误信息：{json.GetProperty("message").GetString()}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            int cnt = dataObj.GetProperty("page").GetProperty("count").GetInt32();
            if (cnt > 0)
            {
                var replies = dataObj.GetProperty("replies");

#if ! NO_DEBUG_FILE
                var fs = new FileStream(DEBUG_PATH, FileMode.Append);
                var stream = new StreamWriter(fs);
                stream.WriteLine(api);
#endif

                foreach (JsonElement item in replies.EnumerateArray())
                {
                    var info = new CommentInfo();
                    info.Id = item.GetProperty("rpid").GetInt64();
                    info.UserId = item.GetProperty("mid").GetInt64();
                    info.Count = item.GetProperty("count").GetInt32();

#if ! NO_DEBUG_FILE
                    {
                        stream.WriteLine(
                            item.GetProperty("member").GetProperty("uname").GetString() + " : " +
                            item.GetProperty("content").GetProperty("message").GetString(), Encoding.UTF8);
                    }
#endif

                    comments.Add(info);
                }

#if ! NO_DEBUG_FILE
                stream.WriteLine();
                stream.Close();
                fs.Close();
#endif
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            comments = null;
        }

        return comments;
    }

    public static List<CommentInfo> GetSubComments(long avid, long rpid, int numPerPage, int page)
    {
        var comments = new List<CommentInfo>();

        try
        {
            string api =
                $"http://api.bilibili.com/x/v2/reply/reply?type=1&oid={avid}&root={rpid}&ps={numPerPage}&pn={page}";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}；错误信息：{json.GetProperty("message").GetString()}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            int cnt = dataObj.GetProperty("page").GetProperty("count").GetInt32();
            if (cnt > 0)
            {
                var replies = dataObj.GetProperty("replies");

#if ! NO_DEBUG_FILE
                var fs = new FileStream(DEBUG_PATH, FileMode.Append);
                var stream = new StreamWriter(fs);
                stream.WriteLine(api);
#endif

                foreach (JsonElement item in replies.EnumerateArray())
                {
                    var info = new CommentInfo();
                    info.Id = item.GetProperty("rpid").GetInt64();
                    info.UserId = item.GetProperty("mid").GetInt64();
                    info.Count = item.GetProperty("count").GetInt32();

#if ! NO_DEBUG_FILE
                    {
                        stream.WriteLine(
                            item.GetProperty("member").GetProperty("uname").GetString() + " : " +
                            item.GetProperty("content").GetProperty("message").GetString(), Encoding.UTF8);
                    }
#endif

                    comments.Add(info);
                }

#if ! NO_DEBUG_FILE
                stream.WriteLine();
                stream.Close();
                fs.Close();
#endif
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            comments = null;
        }

        return comments;
    }
}