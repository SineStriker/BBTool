using System.Net.NetworkInformation;
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
            bool online = false; //是否在线
            while (!online)
            {
                var ping = new Ping();
                var pingReply = await ping.SendPingAsync("https://www.bilibili.com/");
                if (pingReply.Status == IPStatus.Success)
                {
                    online = true;
                    Console.WriteLine("当前在线，已ping通！");
                }
                else
                {
                    Console.WriteLine("不在线，ping不通！");
                }

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