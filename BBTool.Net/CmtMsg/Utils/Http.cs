namespace CmtMsg;

public class Http
{
    public readonly static Dictionary<long, string> SpecialErrors = new Dictionary<long, string>()
    {
        { 21045, "对陌生人最多主动发送一条私信" },
        { 25003, "黑名单" },
    };

    public static string Cookie = "";
}