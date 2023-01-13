namespace BBRsm.Core.RPC;

public static class RSet
{
    public class Request : IRequest
    {
        public string Command { get; set; } = "set";

        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }

    public class Response : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;
    }
}