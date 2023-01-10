using System.Text.Json;
using A180.CoreLib.Text;
using BBTool.Config.Commands;

namespace Somsg.Commands;

public class MyGenCommand : GenConfigCommand
{
    protected override async Task GenerateConfigFile(FileInfo info)
    {
        var conf = new AppConfig
        {
            Message = "你好"
        };

        // 生成默认配置信息后退出
        await AJson.SaveAsync(info.FullName, conf);
    }
}