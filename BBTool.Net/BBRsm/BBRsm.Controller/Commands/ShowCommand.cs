using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;

namespace BBRsm.Controller.Commands;

public class ShowCommand : Command
{
    public readonly Command Users = new("users", "显示所有账户");

    public readonly Command Active = new("active", "显示活跃账户");

    public readonly Command Blocked = new("block", "显示睡眠账户");

    public readonly Command Expired = new("expire", "显示已失效的账户");

    public readonly Command SentUsers = new("recv", "显示已发送消息的用户");

    public readonly Command Videos = new("videos", "显示已搜索的视频");

    public readonly Command Fails = new("fails", "显示失败记录");

    public readonly Command BlackList = new("bl", "显示黑名单");

    public readonly Option<bool> Verbose = new("verbose", "显示完整列表");

    public readonly Argument<long> UserId = new("uid", "账户ID");

    public ShowCommand() : base("show", "显示指定的数据表")
    {
        BlackList.Add(UserId);

        Add(Users);
        Add(Active);
        Add(Blocked);
        Add(Expired);
        Add(SentUsers);
        Add(Videos);
        Add(Fails);
        Add(BlackList);

        Users.SetHandler(UserCommand.ListRoutine);
        Active.SetHandler(UserRoutine);
        Blocked.SetHandler(UserRoutine);
        Expired.SetHandler(UserRoutine);
        SentUsers.SetHandler(UserRoutine);
        Videos.SetHandler(VideosRoutine);
        Fails.SetHandler(FailsRoutine);
        BlackList.SetHandler(BlackListRoutine);

        AddGlobalOption(Verbose);
    }

    private async Task UserRoutine(InvocationContext context)
    {
        var res = context.ParseResult;
        var cmd = res.CommandResult.Command;

        RUser.ListRequest obj;
        if (cmd == Active)
        {
            obj = new RUser.ActiveListRequest();
        }
        else if (cmd == Blocked)
        {
            obj = new RUser.BlockedListRequest();
        }
        else if (cmd == Expired)
        {
            obj = new RUser.ExpiredListRequest();
        }
        else if (cmd == SentUsers)
        {
            obj = new RUser.ReceiversListRequest();
        }
        else
        {
            throw new CommandLineConfigurationException("非法的命令");
        }

        await UserCommand.SendListRequest(obj);
    }

    private async Task VideosRoutine(InvocationContext context)
    {
        var res = context.ParseResult;
        var obj = new RShow.VideoRequest
        {
            Verbose = res.HasOption(Verbose)
        };

        await ClientSend.Post(obj, resp => { });
    }

    private async Task FailsRoutine(InvocationContext context)
    {
        var res = context.ParseResult;
        var obj = new RShow.FailsRequest
        {
            Verbose = res.HasOption(Verbose)
        };

        await ClientSend.Post(obj, resp => { });
    }

    private async Task BlackListRoutine(InvocationContext context)
    {
        var res = context.ParseResult;

        var obj = new RUser.HostileListRequest
        {
            Mid = res.GetValueForArgument(UserId)
        };

        await UserCommand.SendListRequest(obj);
    }

    // private async Task CommonRoutine(InvocationContext context)
    // {
    //     var res = context.ParseResult;
    //     var obj = new RShow.Request
    //     {
    //         Key = res.CommandResult.Command.Name,
    //     };
    //
    //     await ClientSend.Post(obj, resp =>
    //     {
    //         var respObj = resp.FromJson<RShow.Response>();
    //         AStdout.Debug(respObj.Value);
    //     });
    // }
}