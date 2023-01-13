using System.Text.Json.Serialization;

namespace BBRsm.Core.RPC;

public interface IRequest
{
    public string Command { set; get; }
}

public interface IResponse
{
    public int Code { set; get; }

    public string Message { set; get; }
}

public class BaseRequest
{
    public string Command { get; set; } = "";
}

public class BaseResponse
{
    public int Code { set; get; } = 0;

    public string Message { set; get; } = "";
}