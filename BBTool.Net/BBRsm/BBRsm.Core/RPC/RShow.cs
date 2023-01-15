using BBRsm.Core.BiliApiImpl;
using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Core.RPC;

public static class RShow
{
    public class VideoRequest : BaseListRequest
    {
        public override string Command { get; set; } = "show-videos";
    }

    public class VideoResponse : BaseListResponse<VideoInfo>
    {
    }

    public class FailsRequest : BaseListRequest
    {
        public override string Command { get; set; } = "show-fails";
    }

    public class FailsResponse : BaseListResponse<FailAttempt>
    {
    }
}