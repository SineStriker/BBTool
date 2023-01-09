using System.Text.Json;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.LowLevel;

namespace BBTool.Core.BiliApi.User;

public class GetRecentTalk : SimpleRequest
{
    public override string ApiPattern =>
        "https://api.vc.bilibili.com/svr_sync/v1/svr_sync/fetch_session_msgs?session_type=1&talker_id={0}&size={1}";

    public async Task<List<MessageInfo>> Send(long talkerId, int count, string cookie)
    {
        return await GetData(obj =>
            {
                var msgList = new List<MessageInfo>();

                if (obj.TryGetProperty("messages", out var messages) && messages.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in messages.EnumerateArray())
                    {
                        msgList.Add(new MessageInfo
                        {
                            Content = item.GetProperty("content").GetString()!,
                            TimeStamp = item.GetProperty("timestamp").GetInt64(),
                            MessageSeq = item.GetProperty("msg_seqno").GetInt64(),
                        });
                    }
                }

                return msgList;
            },
            () => HttpNew.Get(ImplementUrl(talkerId, count), cookie)
        );
    }
}