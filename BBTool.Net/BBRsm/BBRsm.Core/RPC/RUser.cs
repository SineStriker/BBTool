using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Core.RPC;

public static class RUser
{
    public class AddRequest : IRequest
    {
        public string Command { get; set; } = "user-add";

        public string Cookie { get; set; } = string.Empty;
    }

    public class AddResponse : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;

        public UserInfo? Info { get; set; }
    }

    public class RemoveRequest : IRequest
    {
        public string Command { get; set; } = "user-remove";

        public long Mid { get; set; } = 0;
    }

    public class RemoveResponse : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;
    }

    public class ClearRequest : IRequest
    {
        public string Command { get; set; } = "user-remove";

        public long MidRelated { get; set; } = 0;
    }

    public class ClearResponse : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;
    }

    public class ListRequest : BaseListRequest
    {
        public override string Command { get; set; } = "user-list";
    }

    public class ActiveListRequest : ListRequest
    {
        public override string Command { get; set; } = "user-active";
    }

    public class BlockedListRequest : ListRequest
    {
        public override string Command { get; set; } = "user-blocked";
    }

    public class ExpiredListRequest : ListRequest
    {
        public override string Command { get; set; } = "user-expired";
    }

    public class ReceiversListRequest : ListRequest
    {
        public override string Command { get; set; } = "user-receivers";
    }

    public class HostileListRequest : ListRequest
    {
        public override string Command { get; set; } = "user-blacklist";
    }

    public class ListResponse : BaseListResponse<UserInfo>
    {
        public List<UserInfo> Users
        {
            get => Values;
            set => Values = value;
        }
    }
}