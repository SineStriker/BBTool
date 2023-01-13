using System.ComponentModel;

namespace BBTool.Core.BiliApi.Codes;

public enum SortOrder
{
    [Description("综合排序")] //
    TotalRank,

    [Description("最多点击")] //
    Click,

    [Description("最新发布")] //
    PubDate,

    [Description("最多弹幕")] //
    DM,

    [Description("最多收藏")] //
    STOW,

    [Description("最多评论")] //
    Scores,
}