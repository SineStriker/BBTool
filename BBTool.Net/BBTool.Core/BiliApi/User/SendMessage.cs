using System.ComponentModel;
using A180.CoreLib.Kernel;
using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.Network;

namespace BBTool.Core.BiliApi.User;

public class SendMessage : SimpleRequest
{
    public override string ApiPattern => "https://api.vc.bilibili.com/web_im/v1/web_im/send_msg";

    public async Task<bool> Send(long senderId, long receiverId, string message, string cookie)
    {
        string csrf = ApiUtil.GetCsrfToken(cookie);
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
            { "msg[dev_id]", ApiUtil.GetDevId() },
            { "msg[timestamp]", DateTimeOffset.Now.ToUnixTimeSeconds() },
            { "msg[content]", "{\"content\":\"" + message + "\"}" },
            { "csrf_token", csrf },
            { "csrf", csrf },
        };

        return await GetData(_ => _code == 0, () => HttpWrapper.PostFormUrlEncoded(ImplementUrl(), fields, cookie));
    }
}