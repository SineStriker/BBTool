namespace BBTool.Core.Entities;

public class CommentInfo
{
    public long Id { get; set; }

    public long Mid { get; set; }

    public string UserName { get; set; } = "";

    public string Message { get; set; } = "";

    public int Count { get; set; }
}