namespace BBTool.Core.BiliApi.Entities;

public class QRCode
{
    public class GenerateResult
    {
        /// <summary>
        /// 二维码地址
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// 二维码实例密钥
        /// </summary>
        public string Key { get; set; } = "";
    }

    public class PollResult
    {
        /// <summary>
        /// 轮循返回代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 若扫码成功返回的带 Session 的 URL
        /// </summary>
        public string Url { get; set; } = "";
    }
}