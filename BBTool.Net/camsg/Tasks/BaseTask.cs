using System.Text.Json;
using BBDown.Core;
using BBTool.Core.LowLevel;

namespace Camsg.Tasks;

public class BaseTask
{
    public virtual int TaskId { get; }

    public virtual string TaskLogDir => Global.AppLogDir;

    public virtual string DataPath => Path.Combine(TaskLogDir, $"task_{TaskId}.json");

    public bool DataExists => File.Exists(DataPath);

    public T LoadData<T>()
    {
        return JsonSerializer.Deserialize<T>(File.ReadAllText(DataPath), Sys.UnicodeJsonSerializeOption())!;
    }

    public void SaveData<T>(T data)
    {
        File.WriteAllText(DataPath, JsonSerializer.Serialize(data, Sys.UnicodeJsonSerializeOption()));
    }

    /// <summary>
    /// 睡眠，直到超时或中断
    /// </summary>
    /// <param name="timeout">毫秒数</param>
    /// <returns>超时返回true，中断返回false</returns>
    public bool Sleep(int timeout)
    {
        var bar = new BBDown.ProgressBar();
        var interrupt = false;
        var task = Task.Run(() =>
            {
                var num = timeout / 100;
                // 避免发送请求太快，设置延时
                for (int i = 0; i < num; ++i)
                {
                    // 判断是否中断
                    if (Global.Interrupt != 0)
                    {
                        interrupt = true;
                        return false;
                    }

                    bar.Report((double)i / num);

                    Thread.Sleep(100);
                }

                return true;
            }
        );

        task.Wait();
        bar.Dispose();

        if (interrupt)
        {
            Logger.LogWarn("任务中断");
        }

        return task.Result;
    }
}