namespace GameMain.Rpc
{
    public enum PacketType : byte
    {
        Ptc,
        RpcReq,
        RpcResp,
    }

    public static class RpcState
    {
        public const byte Wait = 1;
        public const byte Delay = 2;
        public const byte Timeout = 3;
        public const byte Done = 4;
    }

    public enum ConnectionState : byte
    {
        Idle,
        Connecting,
        Connected,
        Closed
    }

    public enum NetState : byte
    {
        Connecting, // 连接中
        Connected, // 已连接
        DisConnect, // 连接断开
        ForceClose, // 主动关闭
    }
}