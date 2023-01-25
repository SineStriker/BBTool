using A180.CoreLib.Text.Extensions;
using BBRsm.Core.RPC;

namespace BBRsm.Daemon.HttpHandlers;

public static class ControlHandler
{
    public static async Task<string> StartRespond(string content)
    {
        var respObj = new RControl.ControlResponse();

        return respObj.ToJson();
    }

    public static async Task<string> StopRespond(string content)
    {
        var respObj = new RControl.ControlResponse();

        return respObj.ToJson();
    }

    public static async Task<string> StatusRespond(string content)
    {
        var respObj = new RControl.ControlResponse();

        return respObj.ToJson();
    }
}