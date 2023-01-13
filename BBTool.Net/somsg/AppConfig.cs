using System.ComponentModel;
using BBTool.Config.Files;
using BBTool.Core.BiliApi.Codes;

namespace Somsg;

public class AppConfig : MessageConfig
{
    public static readonly VideoSection DefaultPartition = VideoSection.喵星人;

    public static readonly SortOrder DefaultSortOrder = SortOrder.PubDate;

    /// <summary>
    /// 搜索分区
    /// </summary>
    public int PartitionNum { get; set; } = (int)DefaultPartition;

    /// <summary>
    /// 排序方式
    /// </summary>
    public int SortOrderNum { get; set; } = (int)DefaultSortOrder;
}