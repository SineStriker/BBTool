using BBDown.Core;
using BBTool.Config;
using BBTool.Core.User;

namespace Somsg.Tasks;

public class BatchMessage : BaseTask
{
    public override int TaskId => 3;

    public Data Data { get; set; } = new();

    public async Task<bool> Run(long sender, List<MidNamePair> receivers, string message)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取发送进度...");

            try
            {
                Data = LoadData<Data>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败，错误信息：{e.Message}");
                return false;
            }
        }

        int i = Data.Progress;
        int n = receivers.Count;
        for (; i < n; ++i)
        {
            var receiver = receivers[i];

            var mid = receiver.Mid;
            var uname = receiver.Name;

            var api = new SendMessage();
            await api.Send(sender, mid, message, MessageTool.Cookie);

            var code = api.Code;
            if (code < 0)
            {
                // 必须中断的错误
                Logger.LogError($"致命错误：{api.ErrorMessage}");
                break;
            }

            if (code == (int)SendMessage.ErrorCode.TooFrequent)
            {
                // 频率过高
                Logger.LogError(api.ErrorMessage);
                break;
            }

            if (code > 0)
            {
                // 记录遇到错误的用户
                HashSet<long> list;
                if (!Data.ErrorAttempts.TryGetValue(code, out list))
                {
                    list = new HashSet<long>();
                    Data.ErrorAttempts.Add(code, list);
                }

                list.Add(mid);

                Logger.LogWarn($"{i + 1}/{n} {uname} 发送消息失败：{api.ErrorMessage}");
            }
            else
            {
                Logger.LogWarn($"{i + 1}/{n} {uname} 发送消息成功");
            }

            // 立即更新进度值
            Data.Progress = i + 1;

            // 避免发送请求太快，设置延时
            if (!Sleep(Global.Config.MessageTimeout))
            {
                break;
            }
        }

        // 保存日志
        SaveData(Data);

        if (i < n)
        {
            return false;
        }

        return true;
    }
}