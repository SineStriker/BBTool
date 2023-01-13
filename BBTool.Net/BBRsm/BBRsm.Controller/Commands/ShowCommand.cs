using System.CommandLine;

namespace BBRsm.Controller.Commands;

public class ShowCommand : Command
{
    public readonly Command Users = new("users", "显示所有账户");

    public readonly Command Active = new("active", "显示活跃账户");

    public readonly Command Blocked = new("block", "显示睡眠账户");

    public readonly Command Expired = new("expire", "显示已失效的账户");

    public readonly Command SentUsers = new("recv", "显示已发送消息的用户");

    public readonly Command Videos = new("videos", "显示已搜索的视频");

    public readonly Command Fails = new("fails", "显示失败记录");

    public readonly Command BlackList = new("bl", "显示黑名单");

    public readonly Option<bool> Verbose = new("verbose", "显示完整列表");

    public readonly Argument<long> UserId = new("uid", "账户ID");

    public ShowCommand() : base("show", "显示指定的数据表")
    {
        BlackList.Add(UserId);
        
        Add(Users);
        Add(Active);
        Add(Blocked);
        Add(Expired);
        Add(SentUsers);
        Add(Videos);
        Add(Fails);
        Add(BlackList);
        
        AddGlobalOption(Verbose);
    }
}