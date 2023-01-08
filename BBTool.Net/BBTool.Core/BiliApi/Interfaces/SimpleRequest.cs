using System.Text.Json;
using BBDown.Core;

namespace BBTool.Core;

public abstract class SimpleRequest : IBiliApi
{
    public abstract string ApiPattern { get; }

    public int Code => _code;

    public string ErrorMessage => _errMsg;

    protected T Fail<T>(string err)
    {
        if (_code == 0)
        {
            _code = Int32.MaxValue;
        }

        _errMsg = err;

        return default;
    }

    public void Reset()
    {
        _code = 0;
        _errMsg = "";
    }

    /// <summary>
    /// 通用请求方法
    /// </summary>
    /// <param name="parseData">如何处理 json 返回值中的 data 字段</param>
    /// <param name="doRequest">如何发送请求</param>
    /// <param name="ignoreNonJsonFormat">是否忽略返回的不是 json 的情况</param>
    /// <param name="checkCode">是否检查返回值是否为0</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async Task<T> GetData<T>(Func<JsonElement, T> parseData, Func<Task<string>> doRequest,
        bool ignoreNonJsonFormat = false,
        bool checkCode = true)
    {
        bool jsonParseOver = false;
        try
        {
            var source = await doRequest();

            // 解析为 Json
            var json = JsonDocument.Parse(source).RootElement;
            jsonParseOver = true;

            // 之后的 Json 异常均不忽略
            JsonElement code;
            if (json.TryGetProperty("code", out code))
            {
                _code = code.GetInt32();
            }
            else if (checkCode)
            {
                return Fail<T>("找不到响应代码");
            }

            if (checkCode && _code != 0)
            {
                return Fail<T>(json.GetProperty("message").GetString());
            }

            JsonElement data;
            if (json.TryGetProperty("data", out data))
            {
                return parseData(data);
            }

            return default;
        }
        catch (Exception e)
        {
            if (e is JsonException && !jsonParseOver && ignoreNonJsonFormat)
            {
                Logger.LogDebug("响应内容为非 Json 格式，已忽略");
                return default;
            }

            // 调试模式下输出异常
            if (Global.EnableDebug)
            {
                Console.WriteLine(e);
            }

            return Fail<T>(e.Message);
        }
    }

    protected virtual string ImplementUrl(params object?[] args)
    {
        return string.Format(ApiPattern, args);
    }

    protected int _code = 0;

    protected string _errMsg = "";
}