using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CommandLine.Midwares.Extensions;
using A180.CoreLib.Text;
using BBRsm.Core;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;

namespace BBRsm.Controller.Commands;

public class AppCommand : RootCommand
{
    // 命令
    public readonly GetCommand Get = new();

    public readonly SetCommand Set = new();

    public readonly UserCommand User = new();

    public readonly Command Users = new("users", "显示所有账户");

    public readonly ShowCommand Show = new();

    public readonly StartCommand Start = new();

    public readonly StopCommand Stop = new();

    public readonly Command Status = new("status", "显示任务状态");

    public AppCommand() : base($"{Rsm.AppDesc}，客户端程序")
    {
        Users.SetHandler(UserCommand.ListRoutine);
        Status.SetHandler(StatusRoutine);

        Add(Get);
        Add(Set);
        Add(User);
        Add(Users);
        // Add(Show);
        // Add(Start);
        // Add(Stop);
        // Add(Status);
    }

    private async Task StatusRoutine(InvocationContext context)
    {
        var res = context.ParseResult;
        var obj = new RControl.StatusRequest();

        await ClientSend.Post(obj, resp =>
        {
            AStdout.Debug("OK"); // 成功
        });
    }
}