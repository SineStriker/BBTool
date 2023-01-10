using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace A180.CoreLib.Text;

public static class AJson
{
    public static class Options
    {
        public static JsonSerializerOptions Unicode =>
            new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

        public static JsonSerializerOptions UnicodeIndented =>
            new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), WriteIndented = true, };

        public static JsonSerializerOptions GetUnicode(bool indented = false)
        {
            return indented ? Unicode : UnicodeIndented;
        }
    }

    /// <summary>
    /// 从字符串反序列化
    /// </summary>
    public static T Deserialize<T>(string s)
    {
        return JsonSerializer.Deserialize<T>(s, Options.Unicode)!;
    }

    /// <summary>
    /// 序列化到字符串
    /// </summary>
    public static string Serialize<T>(T obj, bool indented = false)
    {
        return JsonSerializer.Serialize(obj, Options.GetUnicode(indented));
    }

    /// <summary>
    /// 从字节数组反序列化
    /// </summary>
    public static T Deserialize<T>(byte[] utf8Bytes)
    {
        return JsonSerializer.Deserialize<T>(utf8Bytes, Options.Unicode)!;
    }

    /// <summary>
    /// 序列化到字节数组
    /// </summary>
    public static byte[] SerializeToUtf8<T>(T obj, bool indented = false)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, Options.GetUnicode(indented));
    }

    /// <summary>
    /// 从字节数组反序列化（选编码）
    /// </summary>
    public static T Deserialize<T>(byte[] utf8Bytes, Encoding codec)
    {
        return Deserialize<T>(codec.GetString(utf8Bytes));
    }

    /// <summary>
    /// 序列化到字节数组（选编码）
    /// </summary>
    public static byte[] Serialize<T>(T obj, Encoding codec, bool indented = false)
    {
        return codec.GetBytes(Serialize(obj, indented));
    }

    /// <summary>
    /// 从文件反序列化
    /// </summary>
    public static T Load<T>(string path)
    {
        return JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options.Unicode)!;
    }

    public static async Task<T> LoadAsync<T>(string path)
    {
        return JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(path), Options.Unicode)!;
    }

    /// <summary>
    /// 序列化到文件
    /// </summary>
    public static void Save<T>(string path, T obj, bool indented = false)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(obj, Options.GetUnicode(indented)));
    }

    public static async Task SaveAsync<T>(string path, T obj, bool indented = false)
    {
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(obj, Options.GetUnicode(indented)));
    }

    /// <summary>
    /// 从文件反序列化（选编码）
    /// </summary>
    public static T Load<T>(string path, Encoding codec)
    {
        return JsonSerializer.Deserialize<T>(codec.GetString(File.ReadAllBytes(path)), Options.Unicode)!;
    }

    public static async Task<T> LoadAsync<T>(string path, Encoding codec)
    {
        return JsonSerializer.Deserialize<T>(codec.GetString(await File.ReadAllBytesAsync(path)), Options.Unicode)!;
    }

    /// <summary>
    /// 序列化到文件（选编码）
    /// </summary>
    public static void Save<T>(string path, T obj, Encoding codec, bool indented = false)
    {
        File.WriteAllBytes(path, codec.GetBytes(JsonSerializer.Serialize(obj, Options.GetUnicode(indented))));
    }

    public static async Task SaveAsync<T>(string path, T obj, Encoding codec, bool indented = false)
    {
        await File.WriteAllBytesAsync(path,
            codec.GetBytes(JsonSerializer.Serialize(obj, Options.GetUnicode(indented))));
    }
}