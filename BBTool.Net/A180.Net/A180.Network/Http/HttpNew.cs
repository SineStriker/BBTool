﻿using System.Net;

namespace A180.Network.Http;

public static class HttpNew
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
        // Logger.LogDebug($"发送 HTTP Get 请求：{url}");

        // 创建 HTTP 请求
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var headers = request.Headers;
        // headers.TryAddWithoutValidation("User-Agent", Http.UserAgent);
        // headers.TryAddWithoutValidation("Cookie", cookie);
        headers.Add("User-Agent", Http.UserAgent);
        if (!string.IsNullOrEmpty(cookie))
        {
            headers.Add("Cookie", cookie);
        }

        if (headerItems != null)
        {
            foreach (var item in headerItems)
            {
                // headers.TryAddWithoutValidation(item.Key, item.Value);
                headers.Add(item.Key, item.Value);
            }
        }

        // headers.CacheControl = CacheControlHeaderValue.Parse("no-cache");
        // headers.Connection.Clear();

        // 读取响应信息
        // var response = (await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        //     .EnsureSuccessStatusCode();
        // var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var response = await Client.SendAsync(request);
        string htmlCode = await response.Content.ReadAsStringAsync();

        // Logger.LogDebug($"HTTP Get 响应：{htmlCode}");

        return htmlCode;
    }

    public static async Task<string> Post(string url, HttpContent content, string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        // Logger.LogDebug($"发送 HTTP Post 请求：{url}");

        // 创建 HTTP 请求
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        var headers = request.Headers;
        // headers.TryAddWithoutValidation("User-Agent", Http.UserAgent);
        // headers.TryAddWithoutValidation("Cookie", cookie);
        // if (headerItems != null)
        // {
        //     foreach (var item in headerItems)
        //     {
        //         headers.TryAddWithoutValidation(item.Key, item.Value);
        //     }
        // }
        headers.Add("User-Agent", Http.UserAgent);
        if (!string.IsNullOrEmpty(cookie))
        {
            headers.Add("Cookie", cookie);
        }

        if (headerItems != null)
        {
            foreach (var item in headerItems)
            {
                // headers.TryAddWithoutValidation(item.Key, item.Value);
                headers.Add(item.Key, item.Value);
            }
        }

        request.Content = content;

        // 读取响应信息
        // var webResponse = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var response = await Client.SendAsync(request);
        string htmlCode = await response.Content.ReadAsStringAsync();

        // Logger.LogDebug($"HTTP Post 响应：{htmlCode}");

        return htmlCode;
    }

    public static async Task<string> PostFormUrlEncoded<T>(string url, IDictionary<string, T> fields,
        string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        // Logger.LogDebug($"Post 表单：{EasyJson.Serialize(fields)}");
        return await Post(url,
            new FormUrlEncodedContent(fields.ToDictionary(pair => pair.Key, pair => pair.Value!.ToString())),
            cookie,
            headerItems
        );
    }
}