using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.LowLevel;
using GenerateResult = BBTool.Core.BiliApi.Entities.QRCode.GenerateResult;

namespace BBTool.Core.BiliApi.Login;

public class QRCodeGen : SimpleRequest
{
    public override string ApiPattern => "https://passport.bilibili.com/x/passport-login/web/qrcode/generate";

    public async Task<GenerateResult> Send()
    {
        return await GetData(obj => new GenerateResult
            {
                Url = obj.GetProperty("url").GetString()!,
                Key = obj.GetProperty("qrcode_key").GetString()!,
            },
            () => HttpNew.Get(ImplementUrl()));
    }
}