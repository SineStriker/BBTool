namespace A180.CoreLib.Kernel;

public static class Fs
{
    public static bool RemoveDirRecursively(string path)
    {
        var info = new DirectoryInfo(path);
        if (info.Exists)
        {
            info.Delete(true);
            return true;
        }

        return false;
    }

    public static bool RemoveFile(string path)
    {
        var info = new FileInfo(path);
        if (info.Exists)
        {
            info.Delete();
            return true;
        }

        return false;
    }
}