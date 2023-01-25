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

public class BaseRequest : IRequest
{
    public virtual string Command { get; set; } = "";
}

public class BaseResponse : IResponse
{
    public int Code { set; get; } = 0;

    public string Message { set; get; } = "";
}

public class BaseMidRequest : BaseRequest
{
    public long Mid { get; set; } = 0;
}

public class BaseListRequest : BaseMidRequest
{
    public override string Command { get; set; } = "show";

    public bool Verbose { get; set; } = false;
}

public class BaseListResponse<T> : BaseResponse
{
    public List<T> Values { get; set; } = new();

    public int Count { get; set; } = 0;
}

public static class CommmandProtocol
{
    public const string Get = "get";

    public const string Set = "set";

    public const string Start = "start";

    public const string Stop = "stop";

    public const string Status = "status";

    public const string ShowVideos = "show-videos";

    public const string ShowFails = "show-fails";

    public const string UserAdd = "user-add";

    public const string UserRemove = "user-remove";

    public const string UserClear = "user-clear";

    public const string UserAll = "user-list";

    public const string UserActive = "user-active";

    public const string UserBlocked = "user-blocked";

    public const string UserExpired = "user-expired";

    public const string UserReceivers = "user-receivers";

    public const string UserBlackList = "user-blacklist";
}