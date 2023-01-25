using BBRsm.Core.BiliApiImpl;
using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Core.RPC;

public static class RShow
{
    // Videos
    public class VideoRequest : BaseListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.ShowVideos;
    }

    public class VideoResponse : BaseListResponse<VideoInfo>
    {
    }

    // Fails
    public class FailsRequest : BaseListRequest
    {
        public override string Command { get; set; } = CommmandProtocol.ShowFails;
    }

    public class FailsResponse : BaseListResponse<FailAttempt>
    {
    }
}