using A180.CoreLib.Kernel;
using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Tasks;

namespace Somsg.Tasks;

public class CacheTask : BaseTask
{
    public CacheContent Data { get; set; } = new();

    public CacheTask(int tid) : base(tid)
    {
    }

    public async Task<int> Run(Action beforeSave = null)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取参数...");

            try
            {
                Data = await LoadDataAsync<CacheContent>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败：{e.Message}");
                return -1;
            }

            if (string.IsNullOrEmpty(Data.KeyWord))
            {
                Logger.LogWarn("日志中没有关键词，可能发生了错误，无法恢复");
                return -1;
            }

            // 必须使用日志中的关键词
            Global.KeyWord = Data.KeyWord;

            Global.Config.PartitionNum = Data.PartitionNum;

            Global.Config.SortOrderNum = Data.SorOrderNum;

            // 若本次未指定消息内容
            if (string.IsNullOrEmpty(MessageTool.Config.Message))
            {
                if (!string.IsNullOrEmpty(Data.SavedMessage))
                {
                    Logger.Log($"由于没有指定消息内容，使用日志中保存的代替");
                    MessageTool.Config.Message = Data.SavedMessage;
                }
                else
                {
                    Logger.LogWarn("缺少消息内容");
                    return -1;
                }
            }
        }
        else
        {
            var keyword = Global.KeyWord;
            AppConfig.SortOrder order = AppConfig.DefaultSortOrder;
            if (string.IsNullOrEmpty(keyword))
            {
                Logger.LogError("缺少关键词");
                return -1;
            }

            // 保存关键词
            Data.KeyWord = keyword;

            // 搜索方式检查
            if (Enum.IsDefined(typeof(AppConfig.SortOrder), Global.Config.SortOrderNum))
            {
                order = (AppConfig.SortOrder)Global.Config.SortOrderNum;
            }
            else
            {
                Logger.LogWarn($"指定的排序方式不合法，使用{Sys.GetEnumDescription(order)}");
            }

            Data.PartitionNum = Global.Config.PartitionNum;

            Data.SorOrderNum = (int)order;

            // 检查是否有消息内容
            if (string.IsNullOrEmpty(Global.Config.Message))
            {
                Logger.LogWarn("缺少消息内容");
                return -1;
            }

            // 保存消息内容
            Data.SavedMessage = Global.Config.Message;

            if (beforeSave != null)
            {
                beforeSave.Invoke();
            }

            // 保存日志
            await SaveDataAsync(Data);
        }

        return 0;
    }
}