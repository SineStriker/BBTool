using System.Text;

namespace A180.CoreLib.Text.Extensions;

public static class JsonExtensions
{
    /// <summary>
    /// 从字符串反序列化
    /// </summary>
    public static T FromJson<T>(this string s)
    {
        return AJson.Deserialize<T>(s);
    }

    /// <summary>
    /// 序列化到字符串
    /// </summary>
    public static string ToJson<T>(this T obj, bool indented = false)
    {
        return AJson.Serialize(obj, indented);
    }

    /// <summary>
    /// 从字节数组反序列化
    /// </summary>
    public static T FromJson<T>(this byte[] utf8Bytes)
    {
        return AJson.Deserialize<T>(utf8Bytes)!;
    }

    /// <summary>
    /// 序列化到字节数组
    /// </summary>
    public static byte[] ToUtf8Json<T>(this T obj, bool indented = false)
    {
        return AJson.SerializeToUtf8(obj, indented);
    }

    /// <summary>
    /// 从字节数组反序列化（选编码）
    /// </summary>
    public static T FromJson<T>(this byte[] utf8Bytes, Encoding codec)
    {
        return AJson.Deserialize<T>(utf8Bytes, codec);
    }

    /// <summary>
    /// 序列化到字节数组（选编码）
    /// </summary>
    public static byte[] Serialize<T>(this T obj, Encoding codec, bool indented = false)
    {
        return AJson.Serialize(obj, codec, indented);
    }
}