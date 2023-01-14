using System.Runtime.CompilerServices;
using A180.Network.Http;
using BBRsm.Core.BiliApiImpl;
using BBTool.Config;
using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Daemon;

public static class Global
{
    /// <summary>
    /// 服务端配置
    /// </summary>
    public static HttpServer? Server;

    /// <summary>
    /// 全局配置
    /// </summary>
    public static AppConfig Config => (AppConfig)MessageTool.Config;

    public static Dictionary<long, UserAndCookie> Accounts
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => GlobalData.Accounts;
    }

    public static Dictionary<long, int> ActiveAccounts
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => GlobalData.ActiveAccounts;
    }

    public static Dictionary<long, DateTime> BlockedAccounts
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => GlobalData.BlockedAccounts;
    }

    public static LinkedList<VideoInfo> VideoQueue
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => GlobalData.VideoQueue;
    }

    public static Dictionary<long, HashSet<long>> BlackLists
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => GlobalData.BlackLists;
    }
}

internal static class GlobalData
{
    public static Dictionary<long, UserAndCookie> Accounts = new();

    public static Dictionary<long, int> ActiveAccounts = new(); // Value：权重

    public static Dictionary<long, DateTime> BlockedAccounts = new(); // Value：时间
    
    public static LinkedList<VideoInfo> VideoQueue = new();
    
    public static Dictionary<long, HashSet<long>> BlackLists = new();
}