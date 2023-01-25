using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Core.RPC;

public static class RUser
{
    // Add
    public class AddRequest : BaseRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserAdd;

        public string Cookie { get; set; } = string.Empty;
    }

    public class AddResponse : BaseResponse
    {
        public UserInfo? Info { get; set; }
    }

    // Remove
    public class RemoveRequest : BaseMidRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserRemove;
    }

    public class RemoveResponse : BaseResponse
    {
    }

    // Clear
    public class ClearRequest : BaseRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserClear;
    }

    public class ClearResponse : BaseResponse
    {
    }

    // List
    public class ListRequest : BaseListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserAll;
    }

    public class ActiveListRequest : ListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserActive;
    }

    public class BlockedListRequest : ListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserBlocked;
    }

    public class ExpiredListRequest : ListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserExpired;
    }

    public class ReceiversListRequest : ListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserReceivers;
    }

    public class HostileListRequest : ListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.UserBlackList;
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