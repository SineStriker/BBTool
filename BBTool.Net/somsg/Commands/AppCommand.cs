using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CommandLine.Affixes;
using BBTool.Config.Commands;
using BBTool.Config.Commands.Affixes;
using BBTool.Config.Files;

namespace Somsg.Commands;

public class AppCommand : RootCommand
{
    // 参数
    public readonly Argument<string> KeyWord = new("keyword", "搜索关键词");

    // 命令
    public readonly LoginCommand Login = new();

    public readonly LogoutCommand Logout = new();

    public readonly LogStateCommand LogState = new();

    public readonly MyGenCommand GenConfig = new();

    public readonly RecoverCommand Recover = new();

    // 复用选项
    public readonly SearchAffix Message;

    // 控制流转移对象
    private Func<InvocationContext, Task> _routine = BaseAffix.EmptyRoutine;

    public AppCommand() : base($"在 {BBTool.Core.Global.Domain} 搜索关键词，选择特定分区与排序方式，对所有视频的 UP 主发送私信")
    {
        Add(KeyWord);

        Add(Login);
        Add(Logout);
        Add(LogState);
        Add(GenConfig);
        Add(Recover);

        Message = new(this);
        Message.Setup();

        this.SetHandler(Routine);
    }

    public void SetRoutine(Func<InvocationContext, Task> routine)
    {
        _routine = routine;

        Recover.SetRoutine(routine);
    }

    private async Task Routine(InvocationContext context)
    {
        // 设置视频id
        Global.KeyWord = context.ParseResult.GetValueForArgument(KeyWord);

        Message.ResolveResult(context);

        await _routine(context);
    }
}