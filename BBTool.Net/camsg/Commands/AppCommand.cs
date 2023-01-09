using System.CommandLine;
using System.CommandLine.Invocation;
using BBTool.Config.Commands;

namespace Camsg.Commands;

public class AppCommand : RootCommand
{
    // 参数
    public Argument<string> VideoId = new("vid", "视频av号或BV号");

    // 命令
    public LoginCommand Login = new();
    
    public LogoutCommand Logout = new();

    public LogStateCommand LogState = new();

    public GenConfigCommand GenConfig = new();

    public RecoverCommand Recover = new();

    // 复用选项
    private WorkCommandImpl _impl = new();

    public AppCommand() : base("对 Bilibili 指定视频的评论区中的所有用户发送消息")
    {
        Add(VideoId);

        Add(Login);
        Add(Logout);
        Add(LogState);
        Add(GenConfig);
        Add(Recover);
        
        _impl.Setup(this);

        this.SetHandler(Routine);
    }

    private async Task Routine(InvocationContext context)
    {
        _impl.VideoId = context.ParseResult.GetValueForArgument(VideoId);
        
        _impl.Parse(context);
        
        await _impl.Routine(context);
    }
}