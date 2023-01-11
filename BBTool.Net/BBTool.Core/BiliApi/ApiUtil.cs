using System.Text.RegularExpressions;

namespace BBTool.Core.BiliApi;

public static class ApiUtil
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
    
    /// <summary>
    /// 获取设备信息，来自 https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/message/private_msg.md
    /// </summary>
    /// <returns></returns>
    public static string GetDevId()
    {
        char[] b = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        char[] s = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".ToCharArray();
        for (int i = 0; i < s.Length; i++)
        {
            if ('-' == s[i] || '4' == s[i])
            {
                continue;
            }

            int randomInt = new Random().Next(0, 16);
            if ('x' == s[i])
            {
                s[i] = b[randomInt];
            }
            else
            {
                s[i] = b[3 & randomInt | 8];
            }
        }

        return new string(s);
    }
}