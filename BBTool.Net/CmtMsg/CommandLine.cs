using System.Text.Json;

namespace CmtMsg;

public class CommandLine
{
    public CommandLine(string[] args)
    {
        var positionalArgs = new List<string>();
        for (int i = 0; i < args.Length; ++i)
        {
            string arg = args[i];
            string nextArg = i < args.Length - 1 ? args[i + 1] : "";
            try
            {
                if (arg == "-f")
                {
                    // 文件
                    Message = File.ReadAllText(nextArg);
                    i++;
                }
                else if (arg == "--config")
                {
                    // 配置文件
                    ConfigFile conf = JsonSerializer.Deserialize<ConfigFile>(File.ReadAllBytes(nextArg),
                        Sys.UnicodeJsonSerializeOption());
                    Message = conf.Message;
                    GetTimeout = conf.GetTimeout;
                    MessageTimeout = conf.MessageTimeout;
                }
                else if (arg == "--reset-config")
                {
                    // 生成默认配置文件
                    File.WriteAllBytes("");
                }
                else if (arg == "-t1")
                {
                    // 设置消息发送间隔
                    GetTimeout = Int32.Parse(nextArg);
                    i++;
                }
                else if (arg == "-t2")
                {
                    // 设置消息发送间隔
                    MessageTimeout = Int32.Parse(nextArg);
                    i++;
                }
                else if (arg == "--debug")
                {
                    // 输出调试信息
                    IsDebug = true;
                }
                else if (arg == "--recover")
                {
                    // 恢复模式
                    IsRecovery = true;
                }
                else if (arg == "-h" || arg == "--help")
                {
                    // 显示帮助
                    ShowHelp();
                    Exit = true;
                }
                else if (!arg.StartsWith("-"))
                {
                    positionalArgs.Add(arg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Exit = true;
            }
        }

        VideoId = positionalArgs.Any() ? positionalArgs.First() : "";
        Message = positionalArgs.Count > 1 ? positionalArgs[1] : "";
    }

    public bool Exit { get; }

    /// <summary>
    /// 选项和参数
    /// </summary>
    public bool IsEmpty { get; }

    public bool IsRecovery { get; }

    public bool IsDebug { get; }

    public string Message { get; }

    public int GetTimeout { get; }

    public int MessageTimeout { get; }

    public string VideoId { get; }

    /// <summary>
    /// 显示帮助
    /// </summary>
    static public void ShowHelp()
    {
        Console.WriteLine("简介：");
        Console.WriteLine("  {0}", "给指定的B站视频评论区用户批量发送私信。");
        Console.WriteLine();

        Console.WriteLine("用法：");
        Console.WriteLine("  {0}", $"{AppConfig.APP_NAME} <视频id> [选项] [消息内容]");
        Console.WriteLine();

        Console.WriteLine("选项：");
        Console.WriteLine("  {0,-25}{1}", "-f <file>", "使用消息文件，不需要指定消息内容");
        Console.WriteLine("  {0,-25}{1}", "-t1", $"设置获取评论区信息时间间隔（毫秒），默认值{AppConfig.DEFAULT_GET_INTERVAL}");
        Console.WriteLine("  {0,-25}{1}", "-t2", $"设置发送消息时间间隔（毫秒），默认值{AppConfig.DEFAULT_MESSAGE_INTERVAL}");
        Console.WriteLine("  {0,-25}{1}", "--debug", "输出调试信息");
        Console.WriteLine("  {0,-25}{1}", "--recover", "尝试恢复上一次的事务");
        Console.WriteLine("  {0,-25}{1}", "--config <file>", "使用配置文件");
        Console.WriteLine("  {0,-25}{1}", "--reset-config <file>", "生成默认配置文件");
        Console.WriteLine("  {0,-25}{1}", "-h, --help", "显示帮助");
    }

    public class ConfigFile
    {
        public string Message { get; set; } = "你好";

        public int GetTimeout { get; set; } = AppConfig.DEFAULT_GET_INTERVAL;

        public int MessageTimeout { get; set; } = AppConfig.DEFAULT_MESSAGE_INTERVAL;
    }
}