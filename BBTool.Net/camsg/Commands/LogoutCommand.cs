using System.CommandLine;
using BBDown.Core;
using BBTool.Core.User;

namespace Camsg.Commands;

public class LogoutCommand : Command
{
    public LogoutCommand() : base("logout", "退出登录")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine()
    {
        var info = new FileInfo(Global.CookiePath);
        if (!info.Exists)
        {
            Logger.LogWarn("没有找到本地Cookie");
            return;
        }

        var api = new Logout();
        var res = await api.Send(File.ReadAllText(info.FullName));
        if (api.Code != 0)
        {
            Logger.LogError(api.ErrorMessage);
            return;
        }

        if (!res)
        {
            Logger.Log("退出登录响应异常，可能您并未登录或已经退出了");
        }
        else
        {
            Logger.Log("退出登录成功");
        }

        // 删除Cookie
        Logger.Log($"删除本地cookie");
        info.Delete();
    }
}