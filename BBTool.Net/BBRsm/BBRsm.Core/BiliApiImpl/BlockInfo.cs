namespace BBRsm.Core.BiliApiImpl;

public class AccountAndState
{
    public UserAndCookie Account { get; set; }

    public bool Blocked { get; set; }

    public AccountAndState(UserAndCookie account, bool blocked)
    {
        Account = account;
        Blocked = blocked;
    }
}

public class BlockInfo
{
    public DateTime Time { get; set; } = DateTime.Now;

    public UserAndCookie Account { get; set; }

    public BlockInfo(UserAndCookie account)
    {
        Account = account;
    }
}