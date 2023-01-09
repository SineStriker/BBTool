using System.CommandLine;
using BBDown.Core;

namespace BBTool.Config.Commands;

public class LoginCommand : Command
{
    public LoginCommand() : base("login", "使用 Web 扫码登录")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine()
    {
        var task = new LoginTask();
        var cookie = await task.Run();
        if (string.IsNullOrEmpty(cookie))
        {
            // 登录失败
            return;
        }

        // 写入Cookie
        Logger.Log($"保存本地cookie");
        File.WriteAllText(MessageTool.CookiePath, cookie);
    }
}