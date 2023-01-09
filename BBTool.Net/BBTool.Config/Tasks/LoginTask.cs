using BBDown;
using BBDown.Core;
using BBTool.Core;
using BBTool.Core.BiliApi;
using BBTool.Core.BiliApi.Login;
using BBTool.Core.BiliApi.Entities;
using QRCoder;

namespace BBTool.Config;

public class LoginTask : BaseTask
{
    /// <summary>
    /// 临时二维码保存路径
    /// </summary>
    public static string QRCodePath = "qrcode.png";

    /// <summary>
    /// 等待扫码轮循时间
    /// </summary>
    public static int LoginPollTimeout = 2000;

    public async Task<string> Run()
    {
        string cookie = "";

        var qrcodeInfo = new FileInfo(QRCodePath);

        // 结束后删除二维码文件
        var rmQRCode = () =>
        {
            if (qrcodeInfo.Exists)
            {
                qrcodeInfo.Delete();
            }
        };

        using (var guard = new LocalTaskGuard(rmQRCode))
        {
            try
            {
                Logger.Log("获取登录地址...");

                // 向服务器请求二维码
                QRCode.GenerateResult qrcode;
                {
                    var api = new QRCodeGen();
                    qrcode = await api.Send();
                    if (qrcode == null)
                    {
                        Logger.LogError(api.ErrorMessage);
                        return "";
                    }
                }

                Logger.Log("生成二维码...");

                // 生成本地二维码
                QRCodeGenerator qrGenerator = new();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode.Url, QRCodeGenerator.ECCLevel.Q);

                PngByteQRCode pngByteCode = new(qrCodeData);

                // 保存临时二维码文件
                await using (var stream = qrcodeInfo.Create())
                {
                    stream.Write(pngByteCode.GetGraphic(7));
                }

                Global.TempFiles.Add(qrcodeInfo);
                Logger.Log($"生成二维码成功：{QRCodePath}, 请打开并扫描, 或扫描打印的二维码");

                // 显示二维码
                var consoleQRCode = new ConsoleQRCode(qrCodeData);
                consoleQRCode.GetGraphic();

                // 轮循等待
                bool flag = false;
                bool over = false;
                while (!over)
                {
                    // 轮循间隔
                    if (!guard.Sleep(LoginPollTimeout, false))
                    {
                        break;
                    }

                    QRCode.PollResult poll;
                    {
                        var api = new QRCodePoll();
                        poll = await api.Send(qrcode.Key);
                        if (poll == null)
                        {
                            Logger.LogError(api.ErrorMessage);
                            break;
                        }
                    }

                    switch (poll.Code)
                    {
                        case 86038:
                            Logger.LogColor("二维码已过期, 请重新执行登录指令.");
                            over = true;
                            break;

                        case 86090:
                            // 等待确认
                            if (!flag)
                            {
                                Logger.Log("扫码成功, 请确认...");
                                flag = !flag;
                            }

                            break;

                        case 86101:
                            // 等待扫码
                            break;

                        case 0:
                        {
                            var cc = poll.Url;

                            // 扫码成功
                            Logger.Log("登录成功: SESSDATA=" + CookieUtil.GetQueryString("SESSDATA", cc));

                            //导出cookie
                            // File.WriteAllText(Path.Combine(APP_DIR, "BBDown.data"),
                            //     cc[(cc.IndexOf('?') + 1)..].Replace("&", ";"));

                            cookie = cc[(cc.IndexOf('?') + 1)..].Replace("&", ";");

                            over = true;
                            break;
                        }

                        default:
                            Logger.LogColor("非法的状态代码");
                            over = true;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        return cookie;
    }
}