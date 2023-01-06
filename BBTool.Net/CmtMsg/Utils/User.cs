using System.Text.Json;
using BBDown.Core;
using BBDown.Core.Util;

namespace CmtMsg;

public class User
{
    public class UserInfo
    {
        public bool IsLogin { get; set; } = false;

        public long UserId { get; set; }

        public string UserName { get; set; }
    }

    public static UserInfo GetUserInfo()
    {
        var info = new UserInfo();
        try
        {
            var api = "https://api.bilibili.com/x/web-interface/nav";
            var source = HTTPUtil.GetWebSourceAsync(api).Result;
            var json = JsonDocument.Parse(source).RootElement;
            int code = json.GetProperty("code").GetInt32();
            if (code != 0)
            {
                Logger.LogDebug($"错误码：{code}");
                throw new Exception();
            }

            var dataObj = json.GetProperty("data");
            info.IsLogin = dataObj.GetProperty("isLogin").GetBoolean();
            info.UserId = dataObj.GetProperty("mid").GetInt64();
            info.UserName = dataObj.GetProperty("uname").GetString();
        }
        catch (Exception e)
        {
        }

        return info;
    }
}