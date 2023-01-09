using System.CommandLine;
using BBDown.Core;
using BBTool.Config;
using BBTool.Core.User;

namespace BBTool.Config.Commands;

public class LoginCommand : Command
{
    public LoginCommand() : base("login", "使用 Web 扫码登录")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine()
    {
        var api = new Login();
        var cookie = await api.Send();
        if (string.IsNullOrEmpty(cookie))
        {
            Logger.LogError(api.ErrorMessage);
            return;
        }

        // 写入Cookie
        Logger.Log($"保存本地cookie");
        File.WriteAllText(MessageTool.CookiePath, cookie);
    }
}