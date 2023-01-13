using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBRsm.Core.RPC;
using BBTool.Core.Network;

namespace BBRsm.Core.FuncTemplates;

public static class ClientSend
{
    public static async Task Post(object obj, Action<string> handler)
    {
        try
        {
            var resp = await HttpWrapper.PostJson(Rsm.ServerUrl, obj.ToJson());
            var baseRespObj = resp.FromJson<BaseResponse>();
            if (baseRespObj.Code != 0)
            {
                AStdout.Warning(baseRespObj.Message);
            }
            else
            {
                handler.Invoke(resp);
            }
        }
        catch (Exception e)
        {
            if (e is HttpRequestException)
            {
                AStdout.Critical($"请求错误：{e.Message}");
            }
            else
            {
                throw;
            }
        }
    }
}