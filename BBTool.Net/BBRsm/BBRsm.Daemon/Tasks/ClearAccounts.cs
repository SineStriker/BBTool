using BBTool.Config.Tasks;
using BBRsm.Daemon;
using BBDown.Core;
using BBTool.Core.BiliApi.Login;

public class ClearAccounts : BaseTask
{
    public ClearAccounts(int tid = -1) : base(tid)
    {
    }

    public async Task<int> Run()
    {
        var ret = 0;

        while (Global.Accounts.Any())
        {
            var account = Global.Accounts.First().Value;
            Global.RemoveAccount(account.Mid);

            Logger.Log($"正在注销账户：{account.Mid}");

            using (var guard = new LocalTaskGuard())
            {
                while (Global.CurrentAccount == account.Mid)
                {
                    // 等待结束使用账户
                    if (!guard.Sleep(Global.Config.WaitTimeout))
                    {
                        ret = -2;
                        goto exit;
                    }
                }
            }

            var api = new Logout();
            var res = await api.Send(account.Cookie);
            if (api.Code != 0)
            {
                Logger.LogError(api.ErrorMessage);
            }

            if (Global.Accounts.Count == 0)
            {
                break;
            }

            using (var guard = new LocalTaskGuard())
            {
                // 避免发送请求太快，设置延时
                if (!guard.Sleep(Global.Config.MessageTimeout))
                {
                    ret = -2;
                    goto exit;
                }
            }
        }

        Logger.Log("已注销所有账户");

    exit:
        return ret;
    }
}