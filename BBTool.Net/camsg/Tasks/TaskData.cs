using BBTool.Core.BiliApi.Entities;

namespace Camsg.Tasks;

public class TaskBaseInfo
{
    public VideoInfo VideoInfo { get; set; } = new();

    public CommentCount CommentInfo { get; set; } = new();

    public string SavedMessage { get; set; } = "";
}

public class CommentProgress
{
    public int Total { get; set; } = 0;

    public bool Finished { get; set; } = false;

    public List<CommentInfo> Comments { get; set; } = new();
}

public class SubCommentProgress
{
    public bool Finished { get; set; } = false;

    public Dictionary<long, CommentProgress> SubComments { get; set; } = new();
}

public class History
{
    public long Avid { get; set; } = 0;

    public List<long> Users { get; set; } = new();

    public Dictionary<int, HashSet<long>> ErrorAttempts { get; set; } = new();
}