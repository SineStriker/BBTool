using BBRsm.Core.RPC;

namespace BBRsm.Core.FuncTemplates;

public static class ServerParse
{
    public static bool ParseDigit(string input, ref int output, IResponse respObj)
    {
        try
        {
            output = int.Parse(input);
            return true;
        }
        catch (Exception e)
        {
            respObj.Code = -3;
            respObj.Message = "无法解析为数值";
        }

        return false;
    }
    
    public static bool ParseDigit(string input, ref long output, IResponse respObj)
    {
        try
        {
            output = long.Parse(input);
            return true;
        }
        catch (Exception e)
        {
            respObj.Code = -3;
            respObj.Message = "无法解析为数值";
        }

        return false;
    }
}