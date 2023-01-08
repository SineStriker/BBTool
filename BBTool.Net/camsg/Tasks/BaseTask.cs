namespace Camsg.Tasks;

public class BaseTask
{
    public virtual int TaskId { get; }

    public virtual string TaskLogDir { get; }

    public virtual string DataPath => Path.Combine(TaskLogDir, $"task_{TaskId}.json");

    public virtual string LogPath => Path.Combine(TaskLogDir, $"task_{TaskId}.log");

    public BaseTask()
    {
        var dir = new DirectoryInfo(TaskLogDir);
        if (!dir.Exists)
        {
            dir.Create();
        }
    }
}