namespace BBRsm.Core.RPC;

public static class RGet
{
    public class Request : BaseRequest
    {
        public override string Command { get; set; } = CommmandProtocol.Get;

        public string Key { get; set; } = string.Empty;
    }

    public class Response : BaseResponse
    {
        public string Value { get; set; } = string.Empty;
    }
}

public static class GetSetCommandProtocol
{
    public const string Keyword = "keyword";

    public const string Message = "message";

    public const string GetTimeout = "get-timeout";

    public const string GetTimeout_2 = "t1";

    public const string MessageTimeout = "message-timeout";

    public const string MessageTimeout_2 = "t2";

    public const string Section = "section";

    public const string BlockTimeout = "block-timeout";

    public const string SearchTimeout = "search-timeout";

    public const string WaitTimeout = "wait-timeout";
}