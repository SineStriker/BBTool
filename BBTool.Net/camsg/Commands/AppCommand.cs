using System.CommandLine;

namespace Camsg.Commands;

public class AppCommand : RootCommand
{
    // 参数
    public Argument<string> VideoId = new("vid", "视频av号或BV号");

    // 命令
    public LoginCommand Login = new();

    public LogoutCommand Logout = new();

    public GenConfigCommand GenConfig = new();

    public RecoverCommand Recover = new();

    // 复用选项
    private WorkContent _work = new();

    public AppCommand() : base("对 Bilibili 指定视频的评论区中的所有用户发送消息")
    {
        Add(VideoId);

        Add(Login);
        Add(Logout);
        Add(GenConfig);
        Add(Recover);

        _work.Setup(this,
            (opt, context) =>
            {
                opt.VideoId = context.ParseResult.GetValueForArgument(VideoId);
            }
        );
    }
}