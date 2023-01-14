using System.ComponentModel;

namespace BBTool.Core.BiliApi.Codes;

public static class MessageReturn
{
    public enum ErrorCode
    {
        [Description("对陌生人最多主动发送一条私信")] //
        StrangerLimit = 21045,

        [Description("发送信息频率过高")] //
        TooFrequent = 21046,

        [Description("因对方黑名单设置，无法发送私信")] //
        BlackList = 25003,
    }
}