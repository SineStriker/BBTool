namespace BBTool.Core.BiliApi.Entities;

public class SearchVideoResult
{
    /// <summary>
    /// 总页数
    /// </summary>
    public int NumPages { get; set; } = 0;

    /// <summary>
    /// 总条目数
    /// </summary>
    public int NumResults { get; set; } = 0;

    /// <summary>
    /// 视频信息
    /// </summary>
    public List<VideoInfo> Videos { get; set; } = new();
}