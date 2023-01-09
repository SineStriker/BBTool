using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.LowLevel;

using PollResult = BBTool.Core.BiliApi.Entities.QRCode.PollResult;

namespace BBTool.Core.BiliApi.Login;

public class QRCodePoll : SimpleRequest
{
    public override string ApiPattern =>
        "https://passport.bilibili.com/x/passport-login/web/qrcode/poll?qrcode_key={0}";

    public async Task<PollResult> Send(string key)
    {
        return await GetData(obj => new PollResult
        {
            Code = obj.GetProperty("code").GetInt32(),
            Url = obj.GetProperty("url").GetString()!,
        }, () => HttpNew.Get(ImplementUrl(key)));
    }
}