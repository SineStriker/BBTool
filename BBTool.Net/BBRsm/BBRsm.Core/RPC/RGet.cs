namespace BBRsm.Core.RPC;

public static class RGet
{
    public class Request : IRequest
    {
        public string Command { get; set; } = "get";

        public string Key { get; set; } = string.Empty;
    }

    public class Response : IResponse
    {
        public int Code { set; get; } = 0;

        public string Message { set; get; } = "";

        public string Value { get; set; } = string.Empty;
    }
}