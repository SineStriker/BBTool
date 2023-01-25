using A180.CoreLib.Text.Extensions;
using BBRsm.Core.RPC;

namespace BBRsm.Daemon.HttpHandlers;

public static class ShowHandler
{
    public static async Task<string> VideosRespond(string content)
    {
        var respObj = new RShow.VideoResponse();

        return respObj.ToJson();
    }

    public static async Task<string> FailsRespond(string content)
    {
        var respObj = new RShow.FailsResponse();

        return respObj.ToJson();
    }
}