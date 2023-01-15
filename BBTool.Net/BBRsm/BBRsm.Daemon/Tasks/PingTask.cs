using System.Net.NetworkInformation;
using BBDown.Core;
using BBTool.Config.Tasks;

namespace BBRsm.Daemon.Tasks;

public class PingTask : BaseTask
{
    public PingTask(int tid = -1) : base(tid)
    {
    }

    public async Task<int> Run()
    {
        int ret = 0;
        using (var guard = new LocalTaskGuard())
        {
            Logger.Log("等待网络恢复中，使用ping测试B站...");
            while (true)
            {
                var ping = new Ping();
                var pingReply = await ping.SendPingAsync("https://www.bilibili.com/");
                if (pingReply.Status == IPStatus.Success)
                {
                    Console.WriteLine("当前在线，已ping通！");
                    break;
                }
                // else
                // {
                //     Console.WriteLine("不在线，ping不通！");
                // }

                // 设置等待间隔
                if (!guard.Sleep(Global.Config.WaitTimeout))
                {
                    ret = -2;
                    break;
                }
            }
        }

        return ret;
    }
}