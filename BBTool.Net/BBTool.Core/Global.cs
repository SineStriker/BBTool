namespace BBTool.Core;

public static class Global
{
    public const string Domain = "Bilibili";

    /// <summary>
    /// 中间生成的临时文件
    /// </summary>
    public static readonly HashSet<FileSystemInfo> TempFiles = new();
    
    /// <summary>
    /// 是否启用 Debug 模式
    /// </summary>
    public static bool EnableDebug = false;
}