using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Web;
using BBDown.Core;
using BBDown.Core.Util;

namespace CmtMsg;

public class User
{
    public class UserInfo
    {
        public bool IsLogin { get; set; } = false;

        public long UserId { get; set; }

        public string UserName { get; set; }
    }

    public enum SendMessageResult
    {
        Success,
        CSRFNotFound,
        BlackList,
        RequestError,
        HttpError,
    }

    public static UserInfo GetUserInfo()
    {
        var info = new UserInfo();
        try
        {
            var api = "https://api.bilibili.com/x/web-interface/nav";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}；错误信息：{json.GetProperty("message").GetString()}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            info.IsLogin = dataObj.GetProperty("isLogin").GetBoolean();
            info.UserId = dataObj.GetProperty("mid").GetInt64();
            info.UserName = dataObj.GetProperty("uname").GetString();
        }
        catch (Exception e)
        {
        }

        return info;
    }

    public static SendMessageResult SendMessage(long senderId, long receiverId, string content)
    {
        // 查找 CSRF Token
        var csrf = "";
        var match = new Regex("bili_jct=(.*?);").Match(Config.COOKIE);
        if (match.Success)
        {
            csrf = match.Groups[1].Value;
        }
        else
        {
            Logger.LogDebug($"找不到 CSRF Token");
            return SendMessageResult.CSRFNotFound;
        }

        // 建立表单
        var fields = new Dictionary<string, object>()
        {
            { "msg[sender_uid]", senderId },
            { "msg[receiver_id]", receiverId },
            { "msg[receiver_type]", 1 },
            { "msg[msg_type]", 1 },
            { "msg[msg_status]", 0 },
            { "msg[dev_id]", "60845D76-10AC-467C-BA03-71204E4D795E" }, // Sys.GetDevId()
            { "msg[timestamp]", DateTimeOffset.Now.ToUnixTimeSeconds() },
            { "msg[content]", "{\"content\":\"" + content + "\"}" },
            { "csrf_token", csrf },
            { "csrf", csrf },
        };
        var dataStr = string.Join('&',
            fields.Select(pair =>
            {
                return HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value.ToString());
            }).ToList());
        var data = Encoding.UTF8.GetBytes(dataStr);

        try
        {
            var api = "https://api.vc.bilibili.com/web_im/v1/web_im/send_msg";


            // 创建 HTTP 请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36";
            request.Headers.Add("Cookie", Config.COOKIE);

            // 写入正文
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            // 读取相应信息
            var source = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                {
                    source = streamReader.ReadToEnd();
                }
            }

            // var source = HTTPUtil.GetPostResponseAsync(api, data, "application/x-www-form-urlencoded").Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code == 25003)
            {
                Logger.LogWarn($"UID{receiverId}：{json.GetProperty("message").GetString()}");
                return SendMessageResult.BlackList;
            }
            else if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}；错误信息：{json.GetProperty("message").GetString()}");
                return SendMessageResult.RequestError;
            }
        }
        catch (Exception e)
        {
            return SendMessageResult.HttpError;
        }

        return SendMessageResult.Success;
    }
}