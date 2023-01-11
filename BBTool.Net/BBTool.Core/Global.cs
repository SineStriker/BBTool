namespace BBTool.Core;

public static class Global
{
    public static readonly string Domain = "Bilibili";

    public static readonly HashSet<FileSystemInfo> TempFiles = new();
    
    public static bool EnableDebug = false;
}