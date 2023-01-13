using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Core.BiliApiImpl;

public class UserAndCookie : UserInfo
{
    public string Cookie { get; set; } = string.Empty;

    public UserAndCookie()
    {
    }

    public UserAndCookie(UserInfo info, string cookie)
    {
        IsLogin = info.IsLogin;
        Mid = info.Mid;
        UserName = info.UserName;
        VipType = info.VipType;
        Money = info.Money;
        Cookie = cookie;
    }
}