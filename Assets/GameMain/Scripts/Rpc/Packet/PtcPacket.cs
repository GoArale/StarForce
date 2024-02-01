using System;
using System.IO;
using GameFramework;
using Google.Protobuf;
using UnityGameFramework.Runtime;


namespace GameMain.Rpc
{
    public abstract class PtcPacket<T, TProto> : PtcBase
        where T : PtcPacket<T, TProto>, new() where TProto : IMessage, new()
    {
        public static int ProtocolId;
        public readonly TProto m_Data = new();

        public override int PacketId => ProtocolId;

        public static T New()
        {
            // todo test
            // todo ReferencePool.Acquire<T>();
            return new T();
        }
        
        public override PacketBase Create()
        {
            return New();
        }

        protected override void Release(bool isShutdown)
        {
            // todo test
            ReferencePool.Release(this);
        }

        public override bool Decode(byte[] data, int offset, int length)
        {
            try
            {
                m_Data.MergeFrom(data, offset, length);
            }
            catch (Exception e)
            {
                Log.Error($"Ptc:{GetType()} decode failed. packetId:{PacketId} exception:{e}");
                return false;
            }

            return true;
        }

        public override bool Encode(MemoryStream st, uint confirmSequenceId)
        {
            try
            {
                st.WriteByte((byte)(SequenceId >> 24));
                st.WriteByte((byte)(SequenceId >> 16));
                st.WriteByte((byte)(SequenceId >> 8));
                st.WriteByte((byte)(SequenceId));
                
                st.WriteByte((byte)PacketType.Ptc);

                st.WriteByte((byte)(PacketId >> 24));
                st.WriteByte((byte)(PacketId >> 16));
                st.WriteByte((byte)(PacketId >> 8));
                st.WriteByte((byte)(PacketId));

                st.WriteByte((byte)(confirmSequenceId >> 24));
                st.WriteByte((byte)(confirmSequenceId >> 16));
                st.WriteByte((byte)(confirmSequenceId >> 8));
                st.WriteByte((byte)(confirmSequenceId));

                m_Data.WriteTo(st);
            }
            catch (Exception e)
            {
                Log.Error($"Ptc:{GetType()} encode failed. packetId:{PacketId} exception:{e}");
                return false;
            }

            return true;
        }
        
        public override void OnReceive()
        {
            Process();
            // todo event
        }

        protected override void Process()
        {
            throw new NotImplementedException($"Ptc:{GetType()} not implemented Process");
        }

        public override void OnSend()
        {
        }

        public override string GetLogMsg()
        {
            return $"Ptc:{GetType()} packetId:{PacketId} sequenceId:{SequenceId} data:{m_Data}";
        }
    }
}