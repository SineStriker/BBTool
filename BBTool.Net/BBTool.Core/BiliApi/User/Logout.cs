using BBTool.Core.BiliApi;
using BBTool.Core.LowLevel;

namespace BBTool.Core.User;

public class Logout : SimpleRequest
{
    public override string ApiPattern => "http://passport.bilibili.com/login/exit/v2";

    public async Task<bool> Send(string cookie)
    {
        string csrf = CookieUtil.GetCsrfToken(cookie);
        if (csrf == "")
        {
            return Fail<bool>("找不到 CSRF Token");
        }

        // 建立表单
        var fields = new Dictionary<string, object>
        {
            { "biliCSRF", csrf },
        };

        return await GetData(_ => _code == 0, () => HttpNew.PostFormUrlEncoded(ImplementUrl(), fields, cookie), true);
    }
}