namespace BBTool.Core.BiliApi.Entities;

public class MessageInfo
{
    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = "";

    /// <summary>
    /// 时间戳
    /// </summary>
    public long TimeStamp { get; set; } = 0;

    /// <summary>
    /// 不知道什么但觉得有用
    /// </summary>
    public long MessageSeq { get; set; } = 0;
}