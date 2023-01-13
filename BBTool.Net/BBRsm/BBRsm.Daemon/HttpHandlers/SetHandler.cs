using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;

namespace BBRsm.Daemon.HttpHandlers;

public static class SetHandler
{
    public static async Task<string> Respond(string content)
    {
        var respObj = new RSet.Response();

        var obj = content.FromJson<RSet.Request>();

        Logger.Log($"{obj.Key}的值");

        switch (obj.Key)
        {
            case "keyword":
                Global.Config.KeyWord = obj.Value;
                break;

            case "message":
                Global.Config.Message = obj.Value;
                break;

            case "get-timeout":
            case "t1":
            {
                var val = Global.Config.GetTimeout;
                ServerParse.ParseDigit(obj.Value, ref val, respObj);
                Global.Config.GetTimeout = val;
            }
                break;

            case "message-timeout":
            case "t2":
            {
                var val = Global.Config.MessageTimeout;
                ServerParse.ParseDigit(obj.Value, ref val, respObj);
                Global.Config.MessageTimeout = val;
            }
                break;

            case "section":
            {
                var val = Global.Config.PartitionNum;
                ServerParse.ParseDigit(obj.Value, ref val, respObj);
                Global.Config.PartitionNum = val;
            }
                break;

            case "block-timeout":
            {
                var val = Global.Config.BlockTimeout;
                ServerParse.ParseDigit(obj.Value, ref val, respObj);
                Global.Config.BlockTimeout = val;
            }
                break;

            case "search-timeout":
            {
                var val = Global.Config.SearchTimeout;
                ServerParse.ParseDigit(obj.Value, ref val, respObj);
                Global.Config.SearchTimeout = val;
            }
                break;

            case "wait-timeout":
            {
                var val = Global.Config.WaitTimeout;
                ServerParse.ParseDigit(obj.Value, ref val, respObj);
                Global.Config.WaitTimeout = val;
            }
                break;

            default:
                respObj.Code = -2;
                respObj.Message = "意外的变量名";
                break;
        }

        return respObj.ToJson();
    }
}