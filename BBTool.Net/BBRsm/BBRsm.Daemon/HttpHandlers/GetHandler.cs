using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBRsm.Core.RPC;

namespace BBRsm.Daemon.HttpHandlers;

public static class GetHandler
{
    public static async Task<string> Respond(string content)
    {
        var respObj = new RGet.Response();

        var obj = content.FromJson<RGet.Request>();

        Logger.Log($"回复{obj.Key}的值");

        switch (obj.Key)
        {
            case GetSetCommandProtocol.Keyword:
                if (string.IsNullOrEmpty(Global.Config.KeyWord))
                {
                    respObj.Code = 1;
                    respObj.Message = "无关键词";
                }
                else
                {
                    respObj.Value = Global.Config.KeyWord;
                }

                break;

            case GetSetCommandProtocol.Message:
                if (string.IsNullOrEmpty(Global.Config.Message))
                {
                    respObj.Code = 1;
                    respObj.Message = "无消息内容";
                }
                else
                {
                    respObj.Value = Global.Config.Message;
                }

                break;

            case GetSetCommandProtocol.GetTimeout:
            case GetSetCommandProtocol.GetTimeout_2:
                respObj.Value = Global.Config.GetTimeout.ToString();
                break;

            case GetSetCommandProtocol.MessageTimeout:
            case GetSetCommandProtocol.MessageTimeout_2:
                respObj.Value = Global.Config.MessageTimeout.ToString();
                break;

            case GetSetCommandProtocol.Section:
                respObj.Value = Global.Config.PartitionNum.ToString();
                break;

            case GetSetCommandProtocol.BlockTimeout:
                respObj.Value = Global.Config.BlockTimeout.ToString();
                break;

            case GetSetCommandProtocol.SearchTimeout:
                respObj.Value = Global.Config.SearchTimeout.ToString();
                break;

            case GetSetCommandProtocol.WaitTimeout:
                respObj.Value = Global.Config.WaitTimeout.ToString();
                break;

            default:
                respObj.Code = -2;
                respObj.Message = "意外的变量名";
                break;
        }

        return respObj.ToJson();
    }
}