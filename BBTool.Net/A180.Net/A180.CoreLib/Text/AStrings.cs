using System.Text;
using A180.CoreLib.Text.Extensions;

namespace A180.CoreLib.Text;

public static class AStrings
{
    public enum AlignOption
    {
        Left,
        Right,
        Center,
    }

    public static string Elide(string s, int len)
    {
        return s.Length <= len ? s : $"{s[..len]}...";
    }

    public static string Pad(string s, int len)
    {
        int count = s.WideCharCount();
        return len < 0 ? s.PadLeft(-len - count) : s.PadRight(len - count);
    }

    public static string Align(string s, int len, AlignOption option = AlignOption.Left)
    {
        string res = string.Empty;
        switch (option)
        {
            case AlignOption.Left:
                res = Pad(s, Math.Abs(len));
                break;
            case AlignOption.Right:
                res = Pad(s, -Math.Abs(len));
                break;
            case AlignOption.Center:
            {
                var sz = s.Length + s.WideCharCount();
                int l = (len - sz) / 2;
                int r = (len - sz) - l;
                res = (l > 0 ? new string(' ', l) : "") + s + (r > 0 ? new string(' ', r) : "");
            }
                break;
        }

        return res;
    }
}