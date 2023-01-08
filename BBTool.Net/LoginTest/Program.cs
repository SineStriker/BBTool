// See https://aka.ms/new-console-template for more information

using BBDown.Core;
using BBTool.Core;
using BBTool.Core.User;

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
                var api = new Login();
                cookie = api.Send().Result;
                if (string.IsNullOrEmpty(cookie))
                {
                    Logger.LogError(api.ErrorMessage);
                    return 0;
                }
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