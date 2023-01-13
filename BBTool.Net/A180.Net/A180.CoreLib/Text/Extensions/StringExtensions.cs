using System.Text;

namespace A180.CoreLib.Text.Extensions;

public static class StringExtensions
{
    public static byte[] ToUtf8(this string s)
    {
        return Encoding.UTF8.GetBytes(s);
    }

    public static string Elide(this string s, int len)
    {
        return AStrings.Elide(s, len);
    }

    public static string Pad(this string s, int len)
    {
        return AStrings.Pad(s, len);
    }

    public static int WideCharCount(this string s)
    {
        Encoding coding = Encoding.GetEncoding("gb2312");
        int count = 0;

        foreach (char ch in s)
        {
            int byteCount = coding.GetByteCount(ch.ToString());
            if (byteCount == 2)
                count++;
        }

        return count;
    }

    public static int WideLength(this string s)
    {
        return s.Length + s.WideCharCount();
    }

    public static string Align(this string s, int len, AStrings.AlignOption option = AStrings.AlignOption.Left)
    {
        return AStrings.Align(s, len, option);
    }
}