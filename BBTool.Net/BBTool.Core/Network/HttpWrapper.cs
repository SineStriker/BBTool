using System.Net;
using System.Net.Http.Headers;
using A180.CoreLib.Text.Extensions;
using A180.Network.Http;
using BBDown.Core;

namespace BBTool.Core.Network;

public static class HttpWrapper
{
    public static readonly HttpClient Client = new(new HttpClientHandler
    {
        AllowAutoRedirect = true,
        AutomaticDecompression = DecompressionMethods.All,
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    })
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    public static async Task<string> Get(string url, string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        Logger.LogDebug($"发送 HTTP Get 请求：{url}");

        var res = await HttpNew.Get(Client, url, cookie, headerItems);

        Logger.LogDebug($"HTTP Get 响应：{res}");

        return res;
    }

    public static async Task<string> Post(string url, HttpContent content, string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        Logger.LogDebug($"发送 HTTP Post 请求：{url}");

        var res = await HttpNew.Post(Client, url, content, cookie, headerItems);

        Logger.LogDebug($"HTTP Post 响应：{res}");

        return res;
    }

    public static async Task<string> PostFormUrlEncoded<T>(string url, IDictionary<string, T> fields,
        string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        Logger.LogDebug($"Post 表单：{fields.ToJson()}");

        return await HttpNew.PostFormUrlEncoded(Client, url, fields, cookie, headerItems);
    }

    public static async Task<string> PostJson(string url, string json,
        string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        Logger.LogDebug($"Post Json：{json}");

        return await HttpNew.PostJson(Client, url, json, cookie, headerItems);
    }
}