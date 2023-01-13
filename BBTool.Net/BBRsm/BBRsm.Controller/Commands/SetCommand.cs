using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CoreLib.Text;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;

namespace BBRsm.Controller.Commands;

public class SetCommand : Command
{
    public Argument<string> Key = new("key", "参数名");

    public Argument<string> Value = new("value", "参数值");

    public SetCommand() : base("set", "设定参数值")
    {
        Add(Key);
        Add(Value);

        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        var res = context.ParseResult;
        var obj = new RSet.Request
        {
            Key = res.GetValueForArgument(Key),
            Value = res.GetValueForArgument(Value)
        };

        await ClientSend.Post(obj, resp =>
        {
            AStdout.Debug("OK"); // 成功
        });
    }
}