using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;

namespace BBRsm.Controller.Commands;

public class GetCommand : Command
{
    public Argument<string> Key = new("key", "参数名");

    public GetCommand() : base("get", "获取参数值")
    {
        Add(Key);

        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        var res = context.ParseResult;
        var obj = new RGet.Request
        {
            Key = res.GetValueForArgument(Key)
        };

        await ClientSend.Post(obj, resp =>
        {
            var respObj = resp.FromJson<RGet.Response>();
            AStdout.Debug(respObj.Value);
        });
    }
}