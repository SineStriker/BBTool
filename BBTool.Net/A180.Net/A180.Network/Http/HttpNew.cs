using System.Net.Http.Headers;
using System.Text;

namespace A180.Network.Http;

public static class HttpNew
{
    public static async Task<string> Get(HttpClient client, string url, string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        // 创建 HTTP 请求
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var headers = request.Headers;
        headers.TryAddWithoutValidation("User-Agent", Http.UserAgent);
        if (!string.IsNullOrEmpty(cookie))
        {
            headers.TryAddWithoutValidation("Cookie", cookie);
        }

        if (headerItems != null)
        {
            foreach (var item in headerItems)
            {
                headers.TryAddWithoutValidation(item.Key, item.Value);
            }
        }

        // headers.CacheControl = CacheControlHeaderValue.Parse("no-cache");
        // headers.Connection.Clear();

        // 读取响应信息
        var response = await client.SendAsync(request);
        string htmlCode = await response.Content.ReadAsStringAsync();

        return htmlCode;
    }

    public static async Task<string> Post(HttpClient client, string url, HttpContent content, string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        // 创建 HTTP 请求
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        var headers = request.Headers;
        headers.TryAddWithoutValidation("User-Agent", Http.UserAgent);
        if (!string.IsNullOrEmpty(cookie))
        {
            headers.TryAddWithoutValidation("Cookie", cookie);
        }

        if (headerItems != null)
        {
            foreach (var item in headerItems)
            {
                headers.TryAddWithoutValidation(item.Key, item.Value);
            }
        }

        request.Content = content;

        // 读取响应信息
        var response = await client.SendAsync(request);
        string htmlCode = await response.Content.ReadAsStringAsync();

        return htmlCode;
    }

    public static async Task<string> PostFormUrlEncoded<T>(HttpClient client, string url, IDictionary<string, T> fields,
        string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        return await Post(
            client,
            url,
            new FormUrlEncodedContent(fields.ToDictionary(pair => pair.Key, pair => pair.Value!.ToString())),
            cookie,
            headerItems
        );
    }

    public static async Task<string> PostJson(HttpClient client, string url, string json,
        string cookie = "",
        IDictionary<string, string>? headerItems = null)
    {
        // var newHeaderItems = headerItems ?? new Dictionary<string, string>();
        // newHeaderItems.Add("Content-Type", "application/json");

        return await Post(client, url,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cookie,
            headerItems
        );
    }
}