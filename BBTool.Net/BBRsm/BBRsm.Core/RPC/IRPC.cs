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

public class BaseListRequest : IRequest
{
    public virtual string Command { get; set; } = "show";
    
    public long MidRelated { get; set; } = 0;

    public bool Verbose { get; set; } = false;
}

public class BaseListResponse<T> : IResponse
{
    public int Code { get; set; } = 0;

    public string Message { get; set; } = string.Empty;

    public List<T> Values { get; set; } = new();

    public int Count { get; set; } = 0;
}