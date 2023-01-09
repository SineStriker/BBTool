using System.ComponentModel;
using BBTool.Config.Files;

namespace Somsg;

public class AppConfig : MessageConfig
{
    public enum Partition
    {
        动物圈 = 217,
        喵星人 = 218,
    }

    public static int DefaultPartition1 = (int)Partition.动物圈;

    public static int DefaultPartition2 = (int)Partition.喵星人;

    /// <summary>
    /// 默认分区
    /// </summary>
    public int Partition1 { get; set; } = DefaultPartition1;

    /// <summary>
    /// 默认子分区
    /// </summary>
    public int Partition2 { get; set; } = DefaultPartition2;
}