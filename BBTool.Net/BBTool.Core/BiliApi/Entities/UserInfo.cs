using System.ComponentModel;

namespace BBTool.Core.Entities;

public class UserInfo
{
    /// <summary>
    /// 是否登录
    /// </summary>
    public bool IsLogin { get; set; }

    /// <summary>
    /// UID
    /// </summary>
    public long Mid { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = "";

    /// <summary>
    /// Vip 信息
    /// </summary>
    public VipType VipType { get; set; }

    /// <summary>
    /// 硬币数
    /// </summary>
    public double Money { get; set; }

    /// <summary>
    /// 当前等级
    /// </summary>
    public int CurrentLevel { get; set; }

    public static bool IsNullOrOff(UserInfo info)
    {
        return info == null || !info.IsLogin;
    }
}

public enum VipType
{
    [Description("无")] None,

    [Description("月度大会员")] Monthly,

    [Description("年度及以上大会员")] Annual,
}