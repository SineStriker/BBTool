using System.ComponentModel;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace A180.CoreLib.Kernel;

public static class Sys
{
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

    public static string GetEnumDescription(Enum enumValue)
    {
        string value = enumValue.ToString();
        FieldInfo field = enumValue.GetType().GetField(value);
        object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false); //获取描述属性
        if (objs == null || objs.Length == 0) //当描述属性没有时，直接返回名称
            return value;
        DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
        return descriptionAttribute.Description;
    }
}
