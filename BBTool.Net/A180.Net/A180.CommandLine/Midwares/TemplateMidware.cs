using System.CommandLine.Builder;

namespace A180.CommandLine.Midwares;

public class TemplateMidware : BaseMidware
{
    public TemplateMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    public override void Setup()
    {
        Work?.Invoke();
    }

    protected internal Action Work { get; set; } = null;
}