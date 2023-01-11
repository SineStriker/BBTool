using A180.CoreLib.Collections;
using A180.CoreLib.Kernel.Extensions;
using A180.CoreLib.Text;
using BBDown;
using BBDown.Core;

namespace BBTool.Config.Tasks;

public class BaseTask
{
    public int TaskId => _taskId;

    public virtual string TaskLogDir => MessageTool.AppLogDir;

    public virtual string DataPath => Path.Combine(TaskLogDir, $"task_{TaskId}.json");

    public bool DataExists => DataPath.IsFile();

    public void RemoveData() => DataPath.RmFile();

    private int _taskId;

    public BaseTask(int tid = -1)
    {
        _taskId = tid;
    }

    public T LoadData<T>()
    {
        return AJson.Load<T>(DataPath);
    }

    public async Task<T> LoadDataAsync<T>()
    {
        return await AJson.LoadAsync<T>(DataPath);
    }

    public void SaveData<T>(T data)
    {
        AJson.Save(DataPath, data);
    }

    public async Task SaveDataAsync<T>(T data)
    {
        await AJson.SaveAsync(DataPath, data);
    }
}

/// <summary>
/// 其监视的范围可捕获 Ctrl +C，不影响全局
/// </summary>
public class LocalTaskGuard : IDisposable
{
    public LocalTaskGuard(Action? action = null)
    {
        ActionAfterDispose = action;

        Console.CancelKeyPress += InterruptHandler;
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= InterruptHandler;

        ActionAfterDispose?.Invoke();
    }

    protected Action? ActionAfterDispose { get; }

    /// <summary>
    /// 不管有没有全局中断都可继续
    /// </summary>
    protected volatile int LocalInterrupt = 0;

    protected void InterruptHandler(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true; // true: 不导致退出；false: 会导致退出
        LocalInterrupt = 1;
    }

    public PredicateSet Cancelers { get; } = new();

    /// <summary>
    /// 睡眠，直到超时或中断
    /// </summary>
    /// <param name="timeout">毫秒数</param>
    /// <param name="showBar">是否显示进度条</param>
    /// <returns>超时返回true，中断返回false</returns>
    public bool Sleep(int timeout, bool showBar = true)
    {
        var bar = showBar ? new ProgressBar() : null;

        var interrupt = false;
        var task = Task.Run(() =>
            {
                var num = timeout / 100;
                // 避免发送请求太快，设置延时
                for (int i = 0; i < num; ++i)
                {
                    // 判断是否中断
                    if (LocalInterrupt != 0 || MessageTool.Interrupt != 0 || Cancelers.Yes)
                    {
                        interrupt = true;
                        return false;
                    }

                    bar?.Report((double)i / num);

                    Thread.Sleep(100);
                }

                return true;
            }
        );

        task.Wait();

        bar?.Dispose();

        if (interrupt)
        {
            Logger.LogWarn("任务中断");
        }

        return task.Result;
    }
}