using System.Text.Json;
using BBTool.Config.Commands;
using BBTool.Core.LowLevel;

namespace Somsg.Commands;

public class MyGenCommand : GenConfigCommand
{
    protected async override Task GenerateConfigFile(FileInfo info)
    {
        var conf = new AppConfig();
        conf.Message = "你好";

        // 生成默认配置信息后退出
        // File.WriteAllText(info.FullName, JsonSerializer.Serialize(conf, AOT.Json.AppConfigCTX.Type));
        
        File.WriteAllText(info.FullName, JsonSerializer.Serialize(conf, Sys.UnicodeJsonSerializeOption(true)));
    }
}