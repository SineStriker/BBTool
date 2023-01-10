namespace A180.CoreLib.Kernel.Extensions;

public static class FileSystem
{
    public static bool Remove(this FileSystemInfo info)
    {
        if (info.Exists)
        {
            if (info is DirectoryInfo dirInfo)
            {
                dirInfo.Delete(true);
            }
            else
            {
                info.Delete();
            }

            return true;
        }

        return false;
    }

    public static bool Exists(this string path)
    {
        return File.Exists(path);
    }

    public static bool IsFile(this string path)
    {
        return new FileInfo(path).Exists;
    }

    public static bool IsDir(this string path)
    {
        return new DirectoryInfo(path).Exists;
    }

    public static string AbsPath(this string path)
    {
        return new FileInfo(path).FullName;
    }

    public static string FileName(this string path)
    {
        return Path.GetFileName(path);
    }

    public static string BaseName(this string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    public static string? DirName(this string path)
    {
        return Path.GetDirectoryName(path);
    }

    public static bool RmFile(this string path)
    {
        return Fs.RemoveFile(path);
    }

    public static bool MkDir(this string path)
    {
        var info = new DirectoryInfo(path);
        if (info.Exists)
        {
            return false;
        }

        Directory.CreateDirectory(path);
        return true;
    }

    public static bool RmDir(this string path)
    {
        return Fs.RemoveDirRecursively(path);
    }
}