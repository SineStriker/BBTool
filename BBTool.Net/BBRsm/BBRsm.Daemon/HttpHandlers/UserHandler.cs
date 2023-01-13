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
        var cookie = obj.Cookie;

        Logger.Log($"添加账户：{cookie}");

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

            var userAndCookie = new UserAndCookie(user!, cookie);

            // 存内存
            Global.Accounts.Add(user!.Mid, userAndCookie);

            // 存数据库
            var db = RedisHelper.Connection.GetDatabase();
            db.StringSet(RedisHelper.Keys.Accounts + "/" + user.Mid, userAndCookie.ToJson());
        }

        return respObj.ToJson();
    }

    public static async Task<string> RemoveRespond(string content)
    {
        var respObj = new RUser.RemoveResponse();

        var obj = content.FromJson<RUser.RemoveRequest>();
        var userId = obj.Mid;

        Logger.Log($"删除账户：{userId}");

        if (Global.Accounts.TryGetValue(obj.Mid, out var user))
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
                // 删内存
                Global.Accounts.Remove(userId);

                // 删数据库
                var db = RedisHelper.Connection.GetDatabase();
                db.KeyDelete(RedisHelper.Keys.Accounts + "/" + user.Mid);
            }
        }
        else
        {
            respObj.Code = -1;
            respObj.Message = "该用户不在数据库中";
        }

        return respObj.ToJson();
    }

    public static async Task<string> ListRespond(string content)
    {
        Logger.Log($"回复账户列表");

        var respObj = new RUser.ListResponse
        {
            Users = Global.Accounts.Select(item => (UserInfo)item.Value).ToList()
        };

        return respObj.ToJson();
    }
}