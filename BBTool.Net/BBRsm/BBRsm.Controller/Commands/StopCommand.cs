using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CoreLib.Text;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;

namespace BBRsm.Controller.Commands;

public class StopCommand : Command
{
    public Command Search = new("search", "停止搜索");

    public Command Send = new("send", "停止发送消息");

    public StopCommand() : base("stop", "停止某个任务")
    {
        Search.SetHandler(SearchRoutine);
        Send.SetHandler(SendRoutine);

        Add(Search);
        Add(Send);
    }

    private async Task SearchRoutine(InvocationContext context)
    {
        var obj = new RControl.StopRequest
        {
            Task = "search"
        };

        await ClientSend.Post(obj, resp =>
        {
            AStdout.Debug("OK"); // 成功
        });
    }

    private async Task SendRoutine(InvocationContext context)
    {
        var obj = new RControl.StopRequest
        {
            Task = "send"
        };

        await ClientSend.Post(obj, resp =>
        {
            AStdout.Debug("OK"); // 成功
        });
    }
}