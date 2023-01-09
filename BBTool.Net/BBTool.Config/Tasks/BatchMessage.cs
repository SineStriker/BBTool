using BBDown.Core;
using BBTool.Config;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.User;

namespace BBTool.Config.Tasks;

public class BatchMessage : BaseTask
{
    public SendProgress Data { get; set; } = new();

    public BatchMessage(int tid) : base(tid)
    {
    }

    public async Task<bool> Run(long sender, List<MidNamePair> receivers, string message)
    {
        if (MessageTool.RecoveryMode && DataExists)
        {
            Logger.Log("从日志中获取发送进度...");

            try
            {
                Data = await LoadDataAsync<SendProgress>();
            }
            catch (Exception e)
            {
                Logger.LogError($"读取日志失败，错误信息：{e.Message}");
                return false;
            }
        }

        using (var guard = new LocalTaskGuard())
        {
            {
                int i = Data.Progress;
                int n = receivers.Count;
                for (; i < n; ++i)
                {
                    var receiver = receivers[i];

                    var mid = receiver.Mid;
                    var uname = receiver.Name;
                    var unameFormatted = BBTool.Core.LowLevel.Text.PadString(uname, 20);

                    bool skip = false;

                    // 获取最近消息
                    {
                        var api = new GetRecentTalk();
                        var msgList = await api.Send(mid, 20, MessageTool.Cookie);

                        if (api.Code != 0)
                        {
                            Logger.LogError($"获取最近消息失败：{api.ErrorMessage}");
                            break;
                        }

                        if (msgList.Count > 0)
                        {
                            Logger.LogWarn($"{i + 1}/{n} {unameFormatted} 存在最近消息，跳过");
                            skip = true;
                        }
                    }

                    // 发送消息
                    if (!skip)
                    {
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

                            Logger.LogWarn($"{i + 1}/{n} {unameFormatted} 发送消息跳过：{api.ErrorMessage}");
                        }
                        else
                        {
                            Logger.LogWarn($"{i + 1}/{n} {unameFormatted} 发送消息成功");
                        }
                    }

                    // 立即更新进度值
                    Data.Progress = i + 1;

                    // 避免发送请求太快，设置延时
                    if (!guard.Sleep(skip ? MessageTool.Config.GetTimeout : MessageTool.Config.MessageTimeout))
                    {
                        break;
                    }
                }

                // 保存日志
                await SaveDataAsync(Data);

                if (i < n)
                {
                    return false;
                }
            }
        }

        return true;
    }
}