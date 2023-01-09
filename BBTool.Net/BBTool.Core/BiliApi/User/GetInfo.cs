using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Interfaces;
using BBTool.Core.LowLevel;

namespace BBTool.Core.BiliApi.User;

public class GetInfo : SimpleRequest
{
    public override string ApiPattern => "https://api.bilibili.com/x/web-interface/nav";

    public async Task<UserInfo> Send(string cookie = "")
    {
        return await GetData(obj =>
                new UserInfo
                {
                    IsLogin = obj.GetProperty("isLogin").GetBoolean(),
                    Mid = obj.GetProperty("mid").GetInt64(),
                    UserName = obj.GetProperty("uname").GetString()!,
                },
            () => HttpNew.Get(ImplementUrl(), cookie)
        );
    }
}