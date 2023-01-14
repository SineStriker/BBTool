namespace BBRsm.Core.BiliApiImpl;

public class BlockInfo
{
    public DateTime Time { get; set; } = DateTime.Now;
    
    public UserAndCookie Account { get; set; } = new();
}