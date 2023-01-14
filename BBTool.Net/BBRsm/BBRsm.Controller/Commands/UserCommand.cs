using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using A180.CoreLib.Text;
using A180.CoreLib.Text.Extensions;
using BBDown.Core;
using BBRsm.Core.FuncTemplates;
using BBRsm.Core.RPC;
using BBTool.Config;
using BBTool.Config.Tasks;
using ConsoleTables;

namespace BBRsm.Controller.Commands;

public class UserCommand : Command
{
    /// <summary>
    /// 添加账户
    /// </summary>
    public readonly Command AddUser = new("add", "添加账户");

    public readonly Option<string> UseCookie = new("--cookie", "指定Cookie");

    public readonly Option<FileInfo> UseCookieFile = new("--cookie-file", "指定Cookie文件");

    /// <summary>
    /// 删除账户
    /// </summary>
    public readonly Command RemoveUser = new("rm", "删除账户");

    public readonly Argument<long> UserId = new("uid", "用户ID");

    /// <summary>
    /// 显示所有账户
    /// </summary>
    public readonly Command ShowUsers = new("list", "显示所有账户");

    public UserCommand() : base("user", "账户操作")
    {
        AddUser.Add(UseCookie);
        AddUser.Add(UseCookieFile);
        Add(AddUser);

        RemoveUser.Add(UserId);
        Add(RemoveUser);

        AddUser.SetHandler(AddRoutine);
        RemoveUser.SetHandler(RemoveRoutine);
        ShowUsers.SetHandler(ListRoutine);

        this.SetHandler(Routine);
    }

    private async Task AddRoutine(InvocationContext context)
    {
        string cookie = string.Empty;
        var res = context.ParseResult;
        if (res.HasOption(UseCookie))
        {
            cookie = res.GetValueForOption(UseCookie)!;
        }
        else if (res.HasOption(UseCookieFile))
        {
            var info = res.GetValueForOption(UseCookieFile)!;
            cookie = await File.ReadAllTextAsync(info.FullName);
        }
        else
        {
            var task = new LoginTask();
            var ret = await task.Run();
            if (ret != 0)
            {
                // 登录失败
                context.ExitCode = ret;
                return;
            }

            cookie = task.Data;
        }

        var obj = new RUser.AddRequest
        {
            Cookie = cookie
        };

        await ClientSend.Post(obj, resp =>
        {
            var respObj = resp.FromJson<RUser.AddResponse>();
            if (respObj.Info != null)
            {
                AStdout.WriteColor($"{respObj.Info.Mid} {respObj.Info.UserName}");
            }
        });
    }

    private async Task RemoveRoutine(InvocationContext context)
    {
        var obj = new RUser.RemoveRequest
        {
            Mid = context.ParseResult.GetValueForArgument(UserId),
        };

        await ClientSend.Post(obj, resp =>
        {
            AStdout.Debug("OK"); // 成功
        });
    }

    private async Task Routine(InvocationContext context)
    {
        AStdout.Warning("缺少命令");
    }

    public static async Task SendListRequest(RUser.ListRequest req)
    {
        await ClientSend.Post(req, resp =>
        {
            var respObj = resp.FromJson<RUser.ListResponse>();
            if (respObj.Users.Count == 0)
            {
                AStdout.Debug("没有账户");
            }
            else
            {
                var header = new List<string> { "序号", "用户ID", "用户名", "等级" };
                var rows = new List<List<string>>();

                var idx = 0;
                foreach (var item in respObj.Users)
                {
                    var row = new List<string>();
                    row.Add((++idx).ToString());
                    row.Add(item.Mid.ToString());
                    row.Add(item.UserName.ToString());
                    row.Add(item.CurrentLevel.ToString());
                    rows.Add(row);
                }

                ATable.ShowTable(header, rows, new List<int> { 5, 10, 10, 5 }, AStrings.AlignOption.Center);
            }
        });
    }

    public static async Task ListRoutine(InvocationContext context)
    {
        await SendListRequest(new RUser.ListRequest());
    }
}