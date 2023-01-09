namespace BBTool.Core.LowLevel;

public class Text
{
    public static string ElideString(string s, int len)
    {
        return s.Length <= len ? s : $"{s.Substring(0, len)}...";
    }
}