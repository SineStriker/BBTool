using System.CommandLine;
using BBTool.Config;

namespace Somsg.Commands;

public class RecoverCommand : Command
{
    // 复用选项
    private WorkContent _work = new();

    public RecoverCommand() : base("recover", "尝试恢复上一次任务")
    {
        _work.Setup(this, (opt, context) => { MessageTool.RecoveryMode = true; });
    }
}