namespace BBTool.Core.BiliApi.Interfaces;

public interface IBiliApi
{
    public string ApiPattern { get; }

    public int Code { get; }

    public string ErrorMessage { get; }
}