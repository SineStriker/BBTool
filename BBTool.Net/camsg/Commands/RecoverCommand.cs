using System.CommandLine;
using System.CommandLine.Invocation;
using BBTool.Config;

namespace Camsg.Commands;

public class RecoverCommand : Command
{
    // 复用选项
    private WorkCommandImpl _impl = new();

    public RecoverCommand() : base("recover", "尝试恢复上一次任务")
    {
        _impl.Setup(this);

        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        MessageTool.RecoveryMode = true;

        _impl.Parse(context);

        await _impl.Routine(context);
    }
}