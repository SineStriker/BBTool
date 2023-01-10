using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace A180.Network;

public static class Http
{
    public static readonly string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36";

    public static string Get(string url, string cookie = "")
    {
        // Logger.LogDebug($"发送 HTTP Get 请求：{url}");

        // 创建 HTTP 请求
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.UserAgent = UserAgent;
        request.ContentType = "text/html;charset=UTF-8";
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", cookie);
        }

        // 读取响应信息
        string source;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            Stream stream = response.GetResponseStream();
            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                source = streamReader.ReadToEnd();
            }
        }

        // Logger.LogDebug($"HTTP Get 响应：{source}");

        return source;
    }

    public static string PostFormUrlEncoded(string url, byte[] bytes, string cookie = "")
    {
        // Logger.LogDebug($"Post 正文长度：{bytes.Length}");
        // Logger.LogDebug($"发送 HTTP Post 请求：{url}");

        // 创建 HTTP 请求
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = bytes.Length;
        request.UserAgent = UserAgent;
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", cookie);
        }

        // 写入正文
        using (Stream reqStream = request.GetRequestStream())
        {
            reqStream.Write(bytes, 0, bytes.Length);
            reqStream.Close();
        }

        // 读取响应信息
        string source;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            Stream stream = response.GetResponseStream();
            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                source = streamReader.ReadToEnd();
            }
        }

        // Logger.LogDebug($"HTTP Post 响应：{source}");

        return source;
    }

    public static string PostFormUrlEncoded<T>(string url, IDictionary<string, T> fields, string cookie = "")
    {
        // Logger.LogDebug($"Post 表单：{EasyJson.Serialize(fields)}");

        return PostFormUrlEncoded(url, Encoding.UTF8.GetBytes(Http.UrlEncode(fields)), cookie);
    }

    public static string UrlEncode<T>(IDictionary<string, T> fields)
    {
        return string.Join('&',
            fields.Select(pair => HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value!.ToString()))
                .ToList());
    }
}