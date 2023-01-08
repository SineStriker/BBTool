using System.CommandLine;
using BBDown.Core;
using BBTool.Core.User;

namespace Camsg.Commands;

public class LoginCommand : Command
{
    public LoginCommand() : base("login", "使用 Web 扫码登录")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine()
    {
        var api = new Login();
        var cookie = api.Send();
        if (string.IsNullOrEmpty(cookie))
        {
            Logger.LogError(api.ErrorMessage);
            return;
        }

        // 写入Cookie
        File.WriteAllText(Global.CookiePath, cookie);
    }
}