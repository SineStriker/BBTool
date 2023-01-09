﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;

namespace BBTool.Config.Commands.Affixes;

public class BaseAffix
{
    public Command Command { get; } = null;

    /// <summary>
    /// 为 Command 设置
    /// </summary>
    /// <param name="command"></param>
    public BaseAffix(Command command)
    {
        Command = command;
    }

    /// <summary>
    /// 安装
    /// </summary>
    public virtual void Setup()
    {
    }

    /// <summary>
    /// 根据自己的参数，执行一些操作
    /// </summary>
    /// <param name="context"></param>
    public virtual void ResolveResult(InvocationContext context)
    {
    }

    /// <summary>
    /// 处理函数（如果设置的话）
    /// </summary>
    /// <param name="context"></param>
    public virtual async Task SimpleRoutine(InvocationContext context)
    {
    }

    public static async Task EmptyRoutine(InvocationContext context)
    {
    }
}