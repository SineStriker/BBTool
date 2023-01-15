using System.ComponentModel;

namespace BBTool.Core.BiliApi.Codes;

public static class Request
{
    public enum ErrorCode
    {
        [Description("账号未登录")] //
        NotLogin = -101,
    }
}