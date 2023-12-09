using System.IO;
using GameFramework.ObjectPool;
using GameFramework.Timer;

namespace GameMain.RpcNetwork
{
    public abstract class PacketBase : ObjectBase
    {
        // message id
        public uint SequenceId { get; set; }
        public abstract int PacketId { get; }
        
        public abstract PacketBase Create();
        public abstract bool Decode(byte[] data, int offset, int length);
        public abstract bool Encode(MemoryStream st, uint confirmSequence);
        public abstract void Process();
        public abstract void OnReceive();
        public abstract void OnSend();
        public abstract string GetLogMsg();
    }

    public abstract class PtcPacketBase : PacketBase { }

    public abstract class RpcPacketBase : PacketBase
    {
        // public ClientNetModule NetModule;
        public TimerTask Timer;
        public int State = RpcState.Wait;

        public uint RpcSequenceId { get; set; }
        public int TimeoutMS { get; set; }

        public override void OnSend()
        {
            State = RpcState.Wait;
            
            // todo show NetWaiting
            
            
        }
    }
}