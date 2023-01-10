using System.Text.Json;
using System.Web;
using A180.CoreLib.Kernel;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.Network;

namespace BBTool.Core.BiliApi.Search;

/// <summary>
/// 根据关键词进行搜索，返回结果每页20项
/// </summary>
public class SearchVideo : SimpleRequest
{
    public override string ApiPattern =>
        "http://api.bilibili.com/x/web-interface/search/type?search_type=video&keyword={0}&order={1}&tids={2}&page={3}" +
        "&duration=0&from_source=web_search";

    public async Task<SearchVideoResult> Send(
        string keyword,
        string order,
        int tid,
        int page,
        string cookie)
    {
        string keywordEncoded = HttpUtility.UrlEncode(keyword);
        return await GetData(obj =>
            {
                var res = new SearchVideoResult
                {
                    NumPages = obj.GetProperty("numPages").GetInt32(),
                    NumResults = obj.GetProperty("numResults").GetInt32(),
                };

                if (res.NumResults > 0 && obj.TryGetProperty("result", out var result))
                {
                    res.Videos = result.EnumerateArray().Select(
                        item =>
                            new VideoInfo
                            {
                                Avid = item.GetProperty("id").GetInt64(),
                                Mid = item.GetProperty("mid").GetInt64(),
                                UserName = item.GetProperty("author").GetString()!,
                                Title = item.GetProperty("title").GetString()!,
                                Category = item.GetProperty("typename").GetString()!,
                                PublishTime = Sys.GetDateTime(item.GetProperty("pubdate").GetInt32()),
                                Staffs = new(),
                            }
                    ).ToList();
                }

                return res;
            }
            , () => HttpWrapper.Get(ImplementUrl(keywordEncoded, order, tid, page), cookie,
                new Dictionary<string, string>
                {
                    { "Referer", $"https://search.bilibili.com/all?keyword={keywordEncoded}" },
                    { "Origin", "https://search.bilibili.com" }
                }
            ));
    }
}