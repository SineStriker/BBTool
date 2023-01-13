﻿using BBTool.Core.BiliApi.Entities;

namespace BBRsm.Core.RPC;

public static class RUser
{
    public class AddRequest : IRequest
    {
        public string Command { get; set; } = "user-add";

        public string Cookie { get; set; } = string.Empty;
    }

    public class AddResponse : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;

        public UserInfo? Info { get; set; }
    }

    public class RemoveRequest : IRequest
    {
        public string Command { get; set; } = "user-remove";

        public long Mid { get; set; } = 0;
    }

    public class RemoveResponse : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;
    }

    public class ListRequest : IRequest
    {
        public string Command { get; set; } = "user-list";
    }

    public class ListResponse : IResponse
    {
        public int Code { get; set; } = 0;

        public string Message { get; set; } = string.Empty;

        public List<UserInfo> Users { get; set; } = new();
    }
}