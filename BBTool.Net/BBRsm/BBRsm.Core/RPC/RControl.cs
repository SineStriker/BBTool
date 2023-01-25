namespace BBRsm.Core.RPC;

public static class RControl
{
    public class BaseControlRequest : BaseRequest
    {
        public string Task { get; set; } = string.Empty;
    }

    // Start
    public class StartRequest : BaseControlRequest
    {
        public override string Command { get; set; } = CommmandProtocol.Start;
    }

    // Stop
    public class StopRequest : BaseControlRequest
    {
        public override string Command { get; set; } = CommmandProtocol.Stop;
    }

    // Status
    public class StatusRequest : BaseRequest
    {
        public override string Command { get; set; } = CommmandProtocol.Status;
    }

    public class ControlResponse : BaseResponse
    {

    }
}