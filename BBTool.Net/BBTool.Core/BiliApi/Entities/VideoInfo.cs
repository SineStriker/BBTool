namespace BBTool.Core.BiliApi.Entities;

public class VideoInfo
{
    /// <summary>
    /// AV 号
    /// </summary>
    public long Avid { get; set; }

    /// <summary>
    /// UP 主 UID
    /// </summary>
    public long Mid { get; set; }

    /// <summary>
    /// UP 主用户名
    /// </summary>
    public string UserName { get; set; } = "";

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// 分区
    /// </summary>
    public string Category { get; set; } = "";

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// 合作者信息
    /// </summary>
    public List<StaffInfo> Staffs { get; set; } = new();
}

public class StaffInfo
{
    /// <summary>
    /// 合作者分工
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// 合作者 UID
    /// </summary>
    public long Mid { get; set; }

    /// <summary>
    /// 合作者用户名
    /// </summary>
    public string UserName { get; set; } = "";
}