using System.Text.Json;
using BBTool.Core.Entities;
using BBTool.Core.LowLevel;

namespace BBTool.Core.User;

public class GetInfo : SimpleRequest
{
    public override string ApiPattern => "https://api.bilibili.com/x/web-interface/nav";

    public UserInfo Send(string cookie = "")
    {
        return GetData(obj =>
                new UserInfo
                {
                    IsLogin = obj.GetProperty("isLogin").GetBoolean(),
                    Mid = obj.GetProperty("mid").GetInt64(),
                    UserName = obj.GetProperty("uname").GetString(),
                },
            () => Http.Get(ImplementUrl(), cookie)
        );
    }
}