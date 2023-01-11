using System.ComponentModel;
using System.Text.Json.Serialization;
using BBTool.Config.Files;

namespace BBRsm.Daemon;

public class AppConfig : MessageConfig
{
    public enum Partition
    {
        动物圈 = 217,
        喵星人 = 218,
    }

    public enum SortOrder
    {
        [Description("综合排序")] //
        TotalRank,

        [Description("最多点击")] //
        Click,

        [Description("最新发布")] //
        PubDate,

        [Description("最多弹幕")] //
        DM,

        [Description("最多收藏")] //
        STOW,

        [Description("最多评论")] //
        Scores,
    }

    public static readonly Partition DefaultPartition = Partition.喵星人;

    public static readonly SortOrder DefaultSortOrder = SortOrder.PubDate;

    public static readonly long DefaultBlockTimeout = 864000000;

    public static readonly long DefaultSearchTimeout = 7200000;

    public static readonly int DefaultWaitTimeout = 1000;

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