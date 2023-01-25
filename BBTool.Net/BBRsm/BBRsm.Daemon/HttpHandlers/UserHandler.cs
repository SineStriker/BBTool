using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBRsm.Core;
using BBRsm.Core.BiliApiImpl;
using BBRsm.Core.RPC;
using BBTool.Core.BiliApi.Entities;
using BBTool.Core.BiliApi.Login;
using BBTool.Core.BiliApi.User;

namespace BBRsm.Daemon.HttpHandlers;

public static class UserHandler
{
    public static async Task<string> AddRespond(string content)
    {
        var respObj = new RUser.AddResponse();

        var obj = content.FromJson<RUser.AddRequest>();
        Logger.Log($"添加账户：{obj.Cookie}");

        var cookie = obj.Cookie;

        var api = new GetInfo();
        var user = await api.Send(cookie);
        if (UserInfo.IsNullOrOff(user))
        {
            respObj.Code = api.Code == 0 ? -1 : 0;
            respObj.Message = api.ErrorMessage;
        }
        else
        {
            respObj.Info = user;
            Global.AddAccount(new UserAndCookie(user!, cookie));
        }

        return respObj.ToJson();
    }

    public static async Task<string> RemoveRespond(string content)
    {
        var respObj = new RUser.RemoveResponse();

        var obj = content.FromJson<RUser.RemoveRequest>();
        Logger.Log($"删除账户：{obj.Mid}");

        if (Global.LogoutTask.Status == TaskStatus.Running)
        {
            respObj.Code = 1;
            respObj.Message = "正在执行注销";
        }
        else if (Global.CurrentAccount == obj.Mid)
        {
            respObj.Code = 1;
            respObj.Message = "此账户正在使用";
        }
        {
            var userId = obj.Mid;

            if (Global.Accounts.TryGetValue(userId, out var user))
            {
                var api = new Logout();
                var res = await api.Send(user.Cookie);
                if (api.Code != 0)
                {
                    respObj.Code = api.Code;
                    respObj.Message = api.ErrorMessage;
                }

                if (!res)
                {
                    respObj.Code = 1;
                    respObj.Message = "退出登录响应异常，可能您并未登录或已经退出了";
                }
                else
                {
                    Global.RemoveAccount(userId);
                }
            }
            else
            {
                respObj.Code = -1;
                respObj.Message = "该用户不在数据库中";
            }
        }

        return respObj.ToJson();
    }

    public static async Task<string> ClearRespond(string content)
    {
        var respObj = new RUser.ClearResponse();

        Logger.Log($"删除所有账户");
        if (Global.LogoutTask.Status == TaskStatus.Running)
        {
            respObj.Code = 1;
            respObj.Message = "正在执行注销";
        }
        else
        {
            Global.LogoutTask.Start();

            respObj.Code = 1;
            respObj.Message = "开始执行注销任务";
        }
        return respObj.ToJson();
    }

    public static async Task<string> ListRespond(string content)
    {
        Logger.Log($"回复账户列表");

        var respObj = new RUser.ListResponse
        {
            Users = Global.Accounts.Select(item => (UserInfo)item.Value).ToList(),
            Count = Global.Accounts.Count,
        };

        return respObj.ToJson();
    }

    public static async Task<string> ActiveRespond(string content)
    {
        Logger.Log($"回复活跃账户列表");

        var respObj = new RUser.ListResponse
        {
            Users = Global.Accounts.Select(item => (UserInfo)item.Value).ToList(),
            Count = Global.Accounts.Count,
        };

        return respObj.ToJson();
    }

    public static async Task<string> BlockedRespond(string content)
    {
        Logger.Log($"回复睡眠账户列表");

        var respObj = new RUser.ListResponse
        {
            Users = Global.Accounts.Select(item => (UserInfo)item.Value).ToList(),
            Count = Global.Accounts.Count,
        };

        return respObj.ToJson();
    }
}