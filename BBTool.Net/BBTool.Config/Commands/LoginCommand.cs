using System.CommandLine;
using System.CommandLine.Invocation;
using BBDown.Core;
using BBTool.Config.Tasks;

namespace BBTool.Config.Commands;

public class LoginCommand : Command
{
    public LoginCommand() : base("login", "使用 Web 扫码登录")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        var task = new LoginTask();
        var ret = await task.Run();
        if (ret != 0)
        {
            // 登录失败
            context.ExitCode = ret;
            return;
        }

        var cookie = task.Data;

        // 写入Cookie
        Logger.Log($"保存本地cookie");
        await File.WriteAllTextAsync(MessageTool.CookiePath, cookie);
    }
}