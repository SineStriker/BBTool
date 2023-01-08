namespace CmtMsg.Tasks;

public class BaseTask
{
    public int TaskId { get; }

    public string DataPath => Path.Combine(AppConfig.APP_LOG_DIR, $"task_{TaskId}.json");

    public string LogPath => Path.Combine(AppConfig.APP_LOG_DIR, $"task_{TaskId}.log");

    public BaseTask(int id)
    {
        TaskId = id;
    }
}