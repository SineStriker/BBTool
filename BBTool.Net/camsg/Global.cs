﻿using BBTool.Config;
using BBTool.Config.Files;

namespace Camsg;

public static class Global
{
    public static string VideoId = "";

    public static MessageConfig Config => MessageTool.Config;
}