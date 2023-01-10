namespace BBTool.Core;

public static class Global
{
    public static readonly string Domain = "Bilibili";
    
    public static bool EnableDebug = false;

    public static HashSet<FileSystemInfo> TempFiles = new();
}