using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BBDown;
using BBDown.Core;
using BBTool.Core.BiliApi;
using BBTool.Core.LowLevel;
using QRCoder;

namespace BBTool.Core.User;

public class Login : SimpleRequest
{
    /// <summary>
    /// 新版 API
    /// </summary>
    public override string ApiPattern => "https://passport.bilibili.com/x/passport-login/web/qrcode/{0}";

    /// <summary>
    /// 临时二维码保存路径
    /// </summary>
    public static string QRCodePath = "qrcode.png";

    /// <summary>
    /// 等待扫码轮循时间
    /// </summary>
    public static int LoginPollTimeout = 2000;

    protected class QRCodeGenerateResult
    {
        public string Url { get; set; }

        public string Key { get; set; }
    }

    protected class QRCodePollResult
    {
        public int Code { get; set; }

        public string Url { get; set; }
    }

    public string Send()
    {
        string cookie = "";

        try
        {
            Logger.Log("获取登录地址...");

            // 向服务器请求二维码
            QRCodeGenerateResult qrcode = GetData(obj => new QRCodeGenerateResult
                {
                    Url = obj.GetProperty("url").GetString(),
                    Key = obj.GetProperty("qrcode_key").GetString(),
                },
                () => Http.Get(ImplementUrl("generate")));
            if (qrcode == null)
            {
                return "";
            }

            Logger.Log("生成二维码...");

            // 生成本地二维码
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcode.Url, QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode pngByteCode = new(qrCodeData);
            File.WriteAllBytes(QRCodePath, pngByteCode.GetGraphic(7));
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
                Thread.Sleep(LoginPollTimeout);

                // 清除上一次请求的信息
                Reset();

                QRCodePollResult pollRes = GetData(obj => new QRCodePollResult
                {
                    Code = obj.GetProperty("code").GetInt32(),
                    Url = obj.GetProperty("url").GetString(),
                }, () => Http.Get(ImplementUrl($"poll?qrcode_key={qrcode.Key}")));

                if (pollRes == null)
                {
                    break;
                }

                switch (pollRes.Code)
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
                        var cc = pollRes.Url;

                        // 扫码成功
                        Logger.Log("登录成功: SESSDATA=" + CookieUtil.GetQueryString("SESSDATA", cc));

                        //导出cookie
                        // File.WriteAllText(Path.Combine(APP_DIR, "BBDown.data"),
                        //     cc[(cc.IndexOf('?') + 1)..].Replace("&", ";"));

                        // 删除二维码文件
                        if (File.Exists(QRCodePath))
                        {
                            File.Delete(QRCodePath);
                        }

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

        if (string.IsNullOrEmpty(cookie) && string.IsNullOrEmpty(_errMsg))
        {
            return Fail<string>("登录失败");
        }

        return cookie;
    }
}