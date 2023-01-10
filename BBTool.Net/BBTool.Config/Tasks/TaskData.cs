namespace BBTool.Config.Tasks;

public class SendProgress
{
    public int Progress { get; set; } = 0;

    public int SendCount { get; set; } = 0;

    public Dictionary<int, HashSet<long>> ErrorAttempts { get; set; } = new();
}