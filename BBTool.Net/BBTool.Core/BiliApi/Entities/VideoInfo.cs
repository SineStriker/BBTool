namespace BBTool.Core.Entities;

public class VideoInfo
{
    public long Avid { get; set; }

    public string Uploader { get; set; } = "";

    public string Title { get; set; } = "";

    public string Category { get; set; } = "";

    public DateTime PublishTime { get; set; }
}
