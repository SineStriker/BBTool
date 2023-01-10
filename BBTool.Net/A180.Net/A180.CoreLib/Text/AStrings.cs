using System.Text;

namespace A180.CoreLib.Text;

public static class AStrings
{
    public static string Elide(string s, int len)
    {
        return s.Length <= len ? s : $"{s.Substring(0, len)}...";
    }

    public static string Pad(string s, int len)
    {
        Encoding coding = Encoding.GetEncoding("gb2312");
        int count = 0;

        foreach (char ch in s.ToCharArray())
        {
            int byteCount = coding.GetByteCount(ch.ToString());
            if (byteCount == 2)
                count++;
        }

        return len < 0 ? s.PadLeft(-len - count) : s.PadRight(len - count);
    }
}