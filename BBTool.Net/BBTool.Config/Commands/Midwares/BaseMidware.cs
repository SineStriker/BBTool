using System.CommandLine.Builder;

namespace BBTool.Config.Commands.Midwares;

public class BaseMidware
{
    public CommandLineBuilder Builder { get; } = null;

    /// <summary>
    /// 为 CommandLineBuilder 设置
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args">入口命令行参数</param>
    public BaseMidware(CommandLineBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// 安装
    /// </summary>
    public virtual void Setup()
    {
    }
}