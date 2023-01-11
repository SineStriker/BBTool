using System.CommandLine;
using System.CommandLine.Invocation;
using A180.CommandLine.Affixes;
using BBTool.Config.Commands;
using BBTool.Config.Commands.Affixes;
using BBTool.Config.Files;

namespace Camsg.Commands;

public class AppCommand : RootCommand
{
    // 参数
    public readonly Argument<string> VideoId = new("vid", "视频av号或BV号");

    // 命令
    public readonly LoginCommand Login = new();

    public readonly LogoutCommand Logout = new();

    public readonly LogStateCommand LogState = new();

    public readonly GenConfigCommand GenConfig = new();

    public readonly RecoverCommand Recover = new();

    // 复用选项
    public readonly MessageAffix<MessageConfig> Message;

    // 控制流转移对象
    private Func<InvocationContext, Task> _routine = BaseAffix.EmptyRoutine;

    public AppCommand() : base($"对 {BBTool.Core.Global.Domain} 指定视频的评论区中的所有用户发送消息")
    {
        Add(VideoId);

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
        Global.VideoId = context.ParseResult.GetValueForArgument(VideoId);

        Message.ResolveResult(context);

        await _routine(context);
    }
}