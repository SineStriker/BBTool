using A180.CoreLib.Text.Extensions;
using BBRsm.Core.RPC;

namespace BBRsm.Daemon.HttpHandlers;

public static class ShowHandler
{
    public static async Task<string> VideoRespond(string content)
    {
        var respObj = new RShow.VideoRequest();

        return respObj.ToJson();
    }
}