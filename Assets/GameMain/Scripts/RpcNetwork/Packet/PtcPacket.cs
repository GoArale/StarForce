using System;
using System.IO;
using GameFramework;
using Google.Protobuf;
using Unity.VisualScripting;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace GameMain.RpcNetwork
{
    public abstract class PtcPacket<T, TProto> : PtcPacketBase where T : PtcPacket<T, TProto>, new() where TProto : IMessage, new()
    {
        // public static int ProtocolId;
        protected TProto Data;
        
        // private static T New()
        // {
        //     return ReferencePool.Acquire<T>();
        // }

        public override int PacketId { get; }

        public override PacketBase Create()
        {
            // todo test
            return ReferencePool.Acquire<T>();
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
                Data.MergeFrom(data, offset, length);
            }
            catch (Exception e)
            {
                Log.Error($"decode protocol buffer failed. packetId:{PacketId} ptc:{Data.GetType()} exception:{e}");
                return false;
            }

            return true;
        }

        public override bool Encode(MemoryStream st, uint confirmSequence)
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
                
                st.WriteByte((byte)(confirmSequence >> 24));
                st.WriteByte((byte)(confirmSequence >> 16));
                st.WriteByte((byte)(confirmSequence >> 8));
                st.WriteByte((byte)(confirmSequence));
                
                Data.WriteTo(st);
            }
            catch (Exception e)
            {
                Log.Error($"encode protocol buffer failed. packetId:{PacketId} ptc:{Data.GetType()} exception:{e}");
                return false;
            }

            return true;
        }
        
        public override void OnReceive()
        {
            Process();
            // todo event
        }

        public override string GetLogMsg()
        {
            return $"ptc:{GetType()} packetId:{PacketId} sequenceId:{SequenceId} data:{Data}";
        }
    }
}