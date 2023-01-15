namespace BBRsm.Core.RPC;

public static class RControl
{
    public class StartRequest : IRequest
    {
        public string Command { get; set; } = "start";

        public string Task { get; set; } = string.Empty;
    }

    public class StopRequest : IRequest
    {
        public string Command { get; set; } = "stop";

        public string Task { get; set; } = string.Empty;
    }

    public class StatusRequest : IRequest
    {
        public string Command { get; set; } = "status";
    }
}