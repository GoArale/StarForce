using System;

namespace GameMain.Rpc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtocolRegisterAttribute : Attribute
    {
        public int ProtocolId { get; }

        public ProtocolRegisterAttribute(int protocolId)
        {
            ProtocolId = protocolId;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RpcTimeoutAttribute : Attribute
    {
        public int TimeoutMs { get; }

        public RpcTimeoutAttribute(int timeoutMs)
        {
            TimeoutMs = timeoutMs;
        }
    }
}