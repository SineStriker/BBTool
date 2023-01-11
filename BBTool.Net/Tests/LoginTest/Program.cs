// See https://aka.ms/new-console-template for more information

using BBDown.Core;
using BBTool.Config;
using BBTool.Config.Tasks;
using BBTool.Core;
using BBTool.Core.BiliApi.Login;

public static class Program
{
    public static string CookiePath = "cookie.txt";

    public static int Main(string[] args)
    {
        Global.EnableDebug = true;

        var argsSet = args.ToHashSet();

        string cookie = "";

        if (!argsSet.Contains("--no-login"))
        {
            Logger.Log("测试登录");
            {
                var task = new LoginTask();
                if (task.Run().Result != 0)
                {
                    return 0;
                }

                cookie = task.Data;
            }

            File.WriteAllText(CookiePath, cookie);

            Logger.LogColor($"{cookie}");
        }
        else
        {
            cookie = File.ReadAllText(CookiePath);
        }

        Logger.Log("测试登出");
        {
            var api = new Logout();
            api.Send(cookie).Wait();
            if (api.Code != 0)
            {
                Logger.LogError(api.ErrorMessage);
                return 0;
            }
        }

        Logger.Log("测试成功");

        return 0;
    }
}