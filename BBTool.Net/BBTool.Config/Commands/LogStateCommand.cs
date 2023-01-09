using System.CommandLine;
using BBTool.Config.Tasks;

namespace BBTool.Config.Commands;

public class LogStateCommand : Command
{
    public LogStateCommand() : base("status", "查看登录状态")
    {
        this.SetHandler(Routine);
    }

    private async Task Routine()
    {
        var task = new CheckUser();
        if (!await task.Run())
        {
            // 网络错误或用户未登录
        }
    }
}