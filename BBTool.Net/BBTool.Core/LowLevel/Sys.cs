using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BBTool.Core.LowLevel;

public class Sys
{
    public static JsonSerializerOptions UnicodeJsonSerializeOption(bool indented = false)
    {
        var opt = new JsonSerializerOptions();
        opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        opt.WriteIndented = indented;
        return opt;
    }

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

    public static DateTime GetDateTime(int timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

    public static T GetMember<T>(object obj, string key)
    {
        var prop = obj.GetType().GetProperty(key);
        Console.WriteLine($"prop: {prop == null}");
        if (prop != null && prop.GetValue(obj) is T val)
        {
            return val;
        }

        return default;
    }

    public static bool RemoveDirRecursively(string path)
    {
        var info = new DirectoryInfo(path);
        if (info.Exists)
        {
            info.Delete(true);
            return true;
        }

        return false;
    }

    public static bool RemoveFile(string path)
    {
        var info = new FileInfo(path);
        if (info.Exists)
        {
            info.Delete();
            return true;
        }

        return false;
    }
}