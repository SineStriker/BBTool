using BBTool.Core.BiliApi.Entities;

namespace Somsg.Tasks;

public class CacheContent
{
    public string KeyWord { get; set; } = "";

    public int PartitionNum { get; set; } = (int)AppConfig.DefaultPartition;
    
    public int SorOrderNum { get; set; } = (int)AppConfig.DefaultSortOrder;

    public string SavedMessage { get; set; } = "";
}

public class SearchProgress
{
    public bool Finished { get; set; } = false;

    public int CurrentPage { get; set; } = 0;

    public int PagesExpected { get; set; } = 0;

    public int TotalExpected { get; set; } = 0;

    public List<VideoInfo> Videos { get; set; } = new();
}

public class History
{
    public List<long> Avids { get; set; } = new();

    public List<long> Users { get; set; } = new();

    public Dictionary<int, HashSet<long>> ErrorAttempts { get; set; } = new();
}