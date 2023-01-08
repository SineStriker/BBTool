using System.Text.Json;

namespace CmtMsg;

public class AppData
{
    public class CommentList
    {
        public List<Video.CommentInfo> Values { get; set; } = new List<Video.CommentInfo>();
        public bool Over { get; set; } = false;
    }

    public static readonly int NumPerPage = 20;

    public long GetInterval { get; set; }

    public long MessageInterval { get; set; }

    public string VideoId { get; set; } = "";

    public string Message { get; set; } = "";

    public Video.VideoInfo VideoInfo { get; set; } = new Video.VideoInfo();

    public Video.CommentCountInfo CommentCountInfo { get; set; } = new Video.CommentCountInfo();

    public CommentList Comments { get; set; } = new CommentList();

    public Dictionary<long, CommentList> SubComments { get; set; } = new Dictionary<long, CommentList>();

    public Dictionary<int, HashSet<long>> ErrorAttempts = new Dictionary<int, HashSet<long>>();

    public int MessageSent { get; set; } = 0;

    public static AppData Load(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<AppData>(File.ReadAllBytes(path), Sys.UnicodeJsonSerializeOption());
        }
        catch (Exception e)
        {
        }

        return null;
    }

    public void Save(string path)
    {
        File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(this, Sys.UnicodeJsonSerializeOption()));
    }

    public bool IsValid()
    {
        if (!VideoId.Any() || !Message.Any())
        {
            return false;
        }

        if (VideoInfo.Avid < 0 || CommentCountInfo.Root < 0 || CommentCountInfo.Total < 0)
        {
            return false;
        }

        return true;
    }
}