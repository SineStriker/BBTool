using System.ComponentModel;
using System.Reflection;

namespace A180.CoreLib.Kernel;

public static class Sys
{
    public static DateTime GetDateTime(int timeStamp)
    {
        // DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        // long lTime = long.Parse(timeStamp + "0000000");
        // TimeSpan toNow = new TimeSpan(lTime);
        // return dtStart.Add(toNow);
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();
    }

    public static T GetMember<T>(object obj, string key)
    {
        var prop = obj.GetType().GetProperty(key);
        Console.WriteLine($"prop: {prop == null}");
        if (prop != null && prop.GetValue(obj) is T val)
        {
            return val;
        }

        return default!;
    }

    public static string GetEnumDescription(Enum enumValue)
    {
        string value = enumValue.ToString();
        FieldInfo field = enumValue.GetType().GetField(value)!;
        object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false); //获取描述属性
        if (objs == null! || objs.Length == 0) //当描述属性没有时，直接返回名称
            return value;
        DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
        return descriptionAttribute.Description;
    }
}