using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace A180.CoreLib.Text;

public static class AStdout
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(object text)
    {
        Console.WriteLine(text);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string text)
    {
        Console.WriteLine(text);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
        params object[] arg)
    {
        Console.WriteLine(format, arg);
    }

    public static void Critical(object text)
    {
        WriteColor(text, ConsoleColor.Red);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Critical(string text)
    {
        WriteColor(text, ConsoleColor.Red);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Critical([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
        params object[] arg)
    {
        WriteColor(string.Format(format, arg), ConsoleColor.Red);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warning(object text)
    {
        WriteColor(text, ConsoleColor.DarkYellow);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warning(string text)
    {
        WriteColor(text, ConsoleColor.DarkYellow);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warning([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format,
        params object[] arg)
    {
        WriteColor(string.Format(format, arg), ConsoleColor.DarkYellow);
    }

    public static void WriteColor(object text, ConsoleColor color = ConsoleColor.Yellow)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
        Console.WriteLine();
    }
}