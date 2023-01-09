namespace BBTool.Config.Files;

public class MessageConfig
{
    public static int DefaultGetTimeout = 1000;

    public static int DefaultMessageTimeout = 5000;

    public static int NumPerPage = 20; // 每次获取一页评论数

    /// <summary>
    /// 发送普通请求的时间间隔
    /// </summary>
    public int GetTimeout { get; set; } = DefaultGetTimeout;

    /// <summary>
    /// 发送消息的时间间隔
    /// </summary>
    public int MessageTimeout { get; set; } = DefaultMessageTimeout;

    /// <summary>
    /// 要发送的消息内容
    /// </summary>
    public string Message { get; set; } = "";
}