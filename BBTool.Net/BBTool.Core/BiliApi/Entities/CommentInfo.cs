namespace BBTool.Core.BiliApi.Entities;

public class CommentInfo
{
    /// <summary>
    /// 评论 id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 发送者 UID
    /// </summary>
    public long Mid { get; set; }

    /// <summary>
    /// 发送者用户名
    /// </summary>
    public string UserName { get; set; } = "";

    /// <summary>
    /// 评论内容
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// 副评论数
    /// </summary>
    public int Count { get; set; }
}