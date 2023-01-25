namespace BBRsm.Core.RPC;

public static class RSet
{
    public class Request : BaseRequest
    {
        public override string Command { get; set; } = CommmandProtocol.Set;

        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }

    public class Response : BaseResponse
    {
    }
}