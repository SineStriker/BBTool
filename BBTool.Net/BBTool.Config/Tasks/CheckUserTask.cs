using BBDown.Core;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.User;

namespace BBTool.Config;

public class CheckUserTask : BaseTask
{
    public UserInfo Data { get; set; } = null;

    public async Task<bool> Run()
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
                return false;
            }
        }
        
        Logger.LogColor($"用户名：{user.UserName}");
        Logger.LogColor($"用户id：{user.Mid}");

        Data = user;

        return true;
    }
}