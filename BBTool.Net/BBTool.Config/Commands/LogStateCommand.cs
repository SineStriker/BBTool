using System.CommandLine;
using BBDown.Core;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.User;

namespace BBTool.Config.Commands;

public class LogStateCommand : Command
{
    public LogStateCommand() : base("status", "查看登录状态")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine()
    {
        var user = await LoadCookieAndGetUser();
        if (user == null)
        {
            return;
        }

        Logger.LogColor($"用户名：{user.UserName}");
        Logger.LogColor($"用户id：{user.Mid}");
    }

    public static async Task<UserInfo> LoadCookieAndGetUser()
    {
        // 加载 COOKIE
        if (File.Exists(MessageTool.CookiePath))
        {
            Logger.Log("加载本地cookie...");
            MessageTool.Cookie = File.ReadAllText(MessageTool.CookiePath);
        }

        // 检测用户是否登录
        Logger.Log("获取用户信息...");
        UserInfo user;
        {
            var api = new GetInfo();
            user = await api.Send(MessageTool.Cookie);
            if (UserInfo.IsNullOrOff(user))
            {
                Logger.LogWarn(api.ErrorMessage);
                return null;
            }
        }
        return user;
    }
}