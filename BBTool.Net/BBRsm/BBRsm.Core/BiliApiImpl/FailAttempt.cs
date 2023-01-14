namespace BBRsm.Core.BiliApiImpl;

public class FailAttempt
{
    public string UserName { get; set; } = string.Empty;

    public long Mid { get; set; } = 0;

    public int Code { get; set; } = 0;
    
    public string Message { get; set; } = string.Empty;
}