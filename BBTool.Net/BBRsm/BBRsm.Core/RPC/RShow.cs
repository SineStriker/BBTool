namespace BBRsm.Core.RPC;

public static class RShow
{
    public class Request : IRequest
    {
        public string Command { get; set; } = "show";

        public string Key { get; set; } = string.Empty;
    }
}