using System.Text.RegularExpressions;

namespace BBTool.Core.BiliApi;

public class CookieUtil
{
    /// <summary>
    /// 从 Cookie 中提取 CSRF Token
    /// </summary>
    /// <param name="cookie">Cookie字符串</param>
    /// <returns></returns>
    public static string GetCsrfToken(string url)
    {
        Regex re = new Regex("(^|;)?bili_jct=([^;]+)(;|$)?", RegexOptions.Compiled);
        MatchCollection mc = re.Matches(url);
        foreach (Match m in mc.Cast<Match>())
        {
            return m.Result("$2");
        }

        return "";
    }

    /// <summary>
    /// 获取url字符串参数, 返回参数值字符串
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <param name="url">url字符串</param>
    /// <returns></returns>
    public static string GetQueryString(string name, string url)
    {
        Regex re = new Regex("(^|&)?(\\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
        MatchCollection mc = re.Matches(url);
        foreach (Match m in mc.Cast<Match>())
        {
            if (m.Result("$2").Equals(name))
            {
                return m.Result("$3");
            }
        }

        return "";
    }
}