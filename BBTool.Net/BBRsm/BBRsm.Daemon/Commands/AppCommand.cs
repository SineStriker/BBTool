using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CommandLine.Affixes;
using BBRsm.Core;

namespace BBRsm.Daemon.Commands;

public class AppCommand : RootCommand
{
    // 命令
    public readonly RunCommand Run = new();

    public readonly MyGenCommand GenConfig = new();

    // 复用选项
    public readonly ServerAffix Message;

    // 控制流转移对象
    private Func<InvocationContext, Task> _routine = BaseAffix.EmptyRoutine;

    public AppCommand() : base($"{Rsm.AppDesc}，服务端程序")
    {
        Add(Run);
        Add(GenConfig);

        Message = new(this);
        Message.Setup();

        this.SetHandler(Routine);
    }

    public void SetRoutine(Func<InvocationContext, Task> routine)
    {
        Run.SetRoutine(routine);

        _routine = routine;
    }

    private async Task Routine(InvocationContext context)
    {
        Console.WriteLine($"请执行{Run.Name}子命令");
        return;

        Message.ResolveResult(context);
        await _routine(context);
    }
}