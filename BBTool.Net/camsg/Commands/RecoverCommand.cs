using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CommandLine.Affixes;
using BBTool.Config;
using BBTool.Config.Commands.Affixes;
using BBTool.Config.Files;

namespace Camsg.Commands;

public class RecoverCommand : Command
{
    // 复用选项
    public MessageAffix<MessageConfig> Message;

    // 控制流转移对象
    private Func<InvocationContext, Task> _routine = BaseAffix.EmptyRoutine;

    public RecoverCommand() : base("recover", "尝试恢复上一次任务")
    {
        Message = new (this);
        Message.Setup();

        this.SetHandler(Routine);
    }

    public void SetRoutine(Func<InvocationContext, Task> routine)
    {
        _routine = routine;
    }

    private async Task Routine(InvocationContext context)
    {
        // 设置恢复标志
        MessageTool.RecoveryMode = true;

        Message.ResolveResult(context);

        await _routine(context);
    }
}