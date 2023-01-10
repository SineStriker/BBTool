using A180.CoreLib.Kernel;
using A180.Network;
using A180.CoreLib.Text;
using BBDown.Core;

namespace BBTool.Core.Network;

public static class HttpWrapper
{
    public static async Task<string> Get(string url, string cookie = "", IDictionary<string, string> headerItems = null)
    {
        Logger.LogDebug($"发送 HTTP Get 请求：{url}");

        var res = await HttpNew.Get(url, cookie, headerItems);

        Logger.LogDebug($"HTTP Get 响应：{res}");

        return res;
    }

    public static async Task<string> Post(string url, HttpContent content, string cookie = "",
        IDictionary<string, string> headerItems = null)
    {
        Logger.LogDebug($"发送 HTTP Post 请求：{url}");

        var res = await HttpNew.Post(url, content, cookie, headerItems);

        Logger.LogDebug($"HTTP Post 响应：{res}");

        return res;
    }

    public static async Task<string> PostFormUrlEncoded<T>(string url, IDictionary<string, T> fields,
        string cookie = "",
        IDictionary<string, string> headerItems = null)
    {
        Logger.LogDebug($"Post 表单：{AJson.Serialize(fields)}");

        return await Post(url,
            new FormUrlEncodedContent(fields.ToDictionary(pair => pair.Key, pair => pair.Value!.ToString())),
            cookie,
            headerItems
        );
    }
}