using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Camsg.Tasks;

namespace Camsg.AOT;

public partial class Json
{
    // [JsonSerializable(typeof(AppConfig))]
    // public partial class AppConfigCTX : JsonSerializerContext
    // {
    //     public static JsonTypeInfo<AppConfig> Type => Default.AppConfig;
    // }
    //
    // [JsonSerializable(typeof(BatchMessage.TaskData))]
    // public partial class BatchMessageTaskDataCTX : JsonSerializerContext
    // {
    //     public static JsonTypeInfo<BatchMessage.TaskData> Type => Default.AppConfig;
    // }
    //
    // [JsonSerializable(typeof(GetVideo.TaskData))]
    // public partial class GetVideoTaskDataCTX : JsonSerializerContext
    // {
    //     public static JsonTypeInfo<GetVideo.TaskData> Type => Default.AppConfig;
    // }
    //
    // [JsonSerializable(typeof(CommentProgress))]
    // public partial class CommentProgressCTX : JsonSerializerContext
    // {
    //     public static JsonTypeInfo<CommentProgress> Type => Default.AppConfig;
    // }
    //
    // [JsonSerializable(typeof(SubCommentProgress))]
    // public partial class SubCommentProgressCTX : JsonSerializerContext
    // {
    //     public static JsonTypeInfo<SubCommentProgress> Type => Default.AppConfig;
    // }
}