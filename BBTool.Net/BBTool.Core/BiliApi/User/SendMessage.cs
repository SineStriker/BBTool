using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BBTool.Core.BiliApi;
using BBTool.Core.LowLevel;

namespace BBTool.Core.User;

public class SendMessage : SimpleRequest
{
    public override string ApiPattern => "https://api.vc.bilibili.com/web_im/v1/web_im/send_msg";

    public enum ErrorCode
    {
        [Description("对陌生人最多主动发送一条私信")] //
        StrangerLimit = 21045,

        [Description("发送信息频率过高")] //
        TooFrequent = 21046,

        [Description("因对方黑名单设置，无法发送私信")] //
        BlackList = 25003,
    }

    public bool Send(int senderId, int receiverId, string message, string cookie)
    {
        string csrf = CookieUtil.GetCsrfToken(cookie);
        if (csrf == "")
        {
            return Fail<bool>("找不到 CSRF Token");
        }

        // 建立表单
        var fields = new Dictionary<string, object>
        {
            { "msg[sender_uid]", senderId },
            { "msg[receiver_id]", receiverId },
            { "msg[receiver_type]", 1 },
            { "msg[msg_type]", 1 },
            { "msg[msg_status]", 0 },
            { "msg[dev_id]", Sys.GetDevId() },
            { "msg[timestamp]", DateTimeOffset.Now.ToUnixTimeSeconds() },
            { "msg[content]", "{\"content\":\"" + message + "\"}" },
            { "csrf_token", csrf },
            { "csrf", csrf },
        };

        return GetData(_ => _code == 0, () => Http.PostFormUrlEncoded(ImplementUrl(), fields, cookie));
    }
}