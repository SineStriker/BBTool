using System.CommandLine;
using System.CommandLine.Invocation;
using BBTool.Config.Tasks;

namespace BBTool.Config.Commands;

public class LogStateCommand : Command
{
    public LogStateCommand() : base("status", "查看登录状态")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        var task = new CheckUser();
        var ret = await task.Run();
        if (ret != 0)
        {
            // 网络错误或用户未登录
            context.ExitCode = ret;
            return;
        }
    }
}