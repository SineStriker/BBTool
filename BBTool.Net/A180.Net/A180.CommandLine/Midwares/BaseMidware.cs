using System.CommandLine.Builder;

namespace A180.CommandLine.Midwares;

public class BaseMidware
{
    public CommandLineBuilder Builder { get; }

    /// <summary>
    /// 为 CommandLineBuilder 设置
    /// </summary>
    /// <param name="builder">命令行构造器</param>
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

    /// <summary>
    /// 安装后的（为了与无返回函数不同名强行加ed）
    /// </summary>
    /// <returns>安装完毕的命令行构造器</returns>
    public CommandLineBuilder Setuped()
    {
        Setup();
        return Builder;
    }
}