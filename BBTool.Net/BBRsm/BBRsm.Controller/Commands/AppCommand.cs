using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CommandLine.Midwares.Extensions;
using A180.CoreLib.Text;
using BBRsm.Core;

namespace BBRsm.Controller.Commands;

public class AppCommand : RootCommand
{
    // 命令
    public readonly GetCommand Get = new();

    public readonly SetCommand Set = new();

    public readonly UserCommand User = new();

    public readonly Command Users = new("users", "显示所有账户");

    public readonly ShowCommand Show = new();

    public AppCommand() : base($"{Rsm.AppDesc}，客户端程序")
    {
        Users.SetHandler(UserCommand.ListRoutine);
            
        Add(Get);
        Add(Set);
        Add(User);
        Add(Users);
        Add(Show);

        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        AStdout.Warning("缺少命令");
    }
}