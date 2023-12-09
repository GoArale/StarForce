using System;
using Unity.VisualScripting;

namespace GameMain.RpcNetwork
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketRegisterAttribute : Attribute
    {
        public readonly int PacketId;

        public PacketRegisterAttribute(int packetId)
        {
            this.PacketId = packetId;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RpcTimeoutAttribute : Attribute
    {
        public readonly int TimeoutMS;

        public RpcTimeoutAttribute(int timeoutMS)
        {
            this.TimeoutMS = timeoutMS;
        }
    }
}