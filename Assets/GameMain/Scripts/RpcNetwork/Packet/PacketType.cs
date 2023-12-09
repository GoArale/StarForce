namespace GameMain.RpcNetwork
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
        private const byte Delay = 2;
        private const byte Timeout = 3;
        private const byte Done = 4;
    }
}