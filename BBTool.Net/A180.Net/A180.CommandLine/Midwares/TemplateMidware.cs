using System.CommandLine.Builder;

namespace A180.CommandLine.Midwares;

public class TemplateMidware : BaseMidware
{
    internal TemplateMidware(CommandLineBuilder builder) : base(builder)
    {
    }

    public override void Setup()
    {
        Work?.Invoke();
    }

    internal Action? Work { get; set; } = null;
}