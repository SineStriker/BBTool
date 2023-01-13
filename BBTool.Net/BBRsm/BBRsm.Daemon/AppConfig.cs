using System.ComponentModel;
using System.Text.Json.Serialization;
using BBTool.Config.Files;
using BBTool.Core.BiliApi.Codes;

namespace BBRsm.Daemon;

public class AppConfig : MessageConfig
{
    public static readonly VideoSection DefaultPartition = VideoSection.喵星人;

    public static readonly SortOrder DefaultSortOrder = SortOrder.PubDate;

    public static readonly long DefaultBlockTimeout = 864000000;

    public static readonly long DefaultSearchTimeout = 7200000;

    public static readonly int DefaultWaitTimeout = 1000;

    /// <summary>
    /// 搜索关键词
    /// </summary>
    public string KeyWord { get; set; } = "";

    /// <summary>
    /// 搜索分区
    /// </summary>
    public int PartitionNum { get; set; } = (int)DefaultPartition;

    /// <summary>
    /// 排序方式
    /// </summary>
    [JsonIgnore]
    public int SortOrderNum { get; set; } = (int)DefaultSortOrder;

    public long BlockTimeout { get; set; } = DefaultBlockTimeout;

    public long SearchTimeout { get; set; } = DefaultSearchTimeout;

    public int WaitTimeout { get; set; } = DefaultWaitTimeout;
}