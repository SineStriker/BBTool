using System.Text.Json;
using BBDown;
using BBDown.Core;
using BBTool.Config;
using BBTool.Core.LowLevel;

namespace BBTool.Config.Tasks;

public class BaseTask
{
    public int TaskId => _taskId;

    public virtual string TaskLogDir => MessageTool.AppLogDir;

    public virtual string DataPath => Path.Combine(TaskLogDir, $"task_{TaskId}.json");

    public bool DataExists => File.Exists(DataPath);

    public void RemoveData() => Sys.RemoveFile(DataPath);

    private int _taskId = -1;

    public BaseTask(int tid = -1)
    {
        _taskId = tid;
    }

    public T LoadData<T>()
    {
        return JsonSerializer.Deserialize<T>(File.ReadAllText(DataPath), Sys.UnicodeJsonSerializeOption())!;
    }

    public void SaveData<T>(T data)
    {
        File.WriteAllText(DataPath, JsonSerializer.Serialize(data, Sys.UnicodeJsonSerializeOption()));
    }

    public async Task<T> LoadDataAsync<T>()
    {
        return JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(DataPath), Sys.UnicodeJsonSerializeOption())!;
    }

    public async Task SaveDataAsync<T>(T data)
    {
        await File.WriteAllTextAsync(DataPath, JsonSerializer.Serialize(data, Sys.UnicodeJsonSerializeOption()));
    }
}

/// <summary>
/// 其监视的范围可捕获 Ctrl +C，不影响全局
/// </summary>
public class LocalTaskGuard : IDisposable
{
    public LocalTaskGuard(Action action = null)
    {
        ActionAfterDispose = action;

        Console.CancelKeyPress += InterruptHandler;
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= InterruptHandler;
        if (ActionAfterDispose != null)
        {
            ActionAfterDispose.Invoke();
        }
    }

    protected Action ActionAfterDispose { get; }

    /// <summary>
    /// 不管有没有全局中断都可继续
    /// </summary>
    protected volatile int LocalInterrupt = 0;

    protected void InterruptHandler(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true; // true: 不导致退出；false: 会导致退出
        LocalInterrupt = 1;
    }

    /// <summary>
    /// 睡眠，直到超时或中断
    /// </summary>
    /// <param name="timeout">毫秒数</param>
    /// <returns>超时返回true，中断返回false</returns>
    public bool Sleep(int timeout, bool showBar = true)
    {
        ProgressBar bar = showBar ? new ProgressBar() : null;

        var interrupt = false;
        var task = Task.Run(() =>
            {
                var num = timeout / 100;
                // 避免发送请求太快，设置延时
                for (int i = 0; i < num; ++i)
                {
                    // 判断是否中断
                    if (LocalInterrupt != 0 || MessageTool.Interrupt != 0)
                    {
                        interrupt = true;
                        return false;
                    }

                    if (bar != null)
                    {
                        bar.Report((double)i / num);
                    }

                    Thread.Sleep(100);
                }

                return true;
            }
        );

        task.Wait();

        if (bar != null)
        {
            bar.Dispose();
        }

        if (interrupt)
        {
            Logger.LogWarn("任务中断");
        }

        return task.Result;
    }
}