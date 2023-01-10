namespace A180.CoreLib.Text.Extensions;

public static class StringExtensions
{
    public static string Elide(this string s, int len)
    {
        return AStrings.Elide(s, len);
    }
    
    public static string Pad(this string s, int len)
    {
        return AStrings.Pad(s, len);
    }
}