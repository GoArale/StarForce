using System;
using System.IO;
using System.Threading;
using GameFramework;
using Google.Protobuf;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public abstract class RpcPacket<T, TReq, TRsp> : RpcBase where T : RpcPacket<T, TReq, TRsp>, new()
        where TReq : IMessage, new()
        where TRsp : IMessage, new()
    {
        public static int ProtocolId;
        public static int TimeoutMs = 5000;
        public override int PacketId => ProtocolId;
        protected override int Timeout => TimeoutMs;

        public readonly TReq m_Req = new();
        public readonly TRsp m_Rsp = new();

        private int m_ErrorCode;

        public static T New()
        {
            return new T();
            //return ReferencePool.Acquire<T>();
        }

        // todo 这里要改成static 上面的New方法
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
                m_ErrorCode = (data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) +
                              data[offset + 3];
                m_Rsp.MergeFrom(data, offset + 4, length - 4);
            }
            catch (Exception e)
            {
                Log.Error($"Rpc:{GetType()} decode failed. packetId:{PacketId} exception:{e}");
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
                st.WriteByte((byte)(SequenceId >> 16));
                st.WriteByte((byte)SequenceId);

                st.WriteByte((byte)PacketType.RpcReq);

                st.WriteByte((byte)(PacketId >> 24));
                st.WriteByte((byte)(PacketId >> 16));
                st.WriteByte((byte)(PacketId >> 8));
                st.WriteByte((byte)PacketId);

                st.WriteByte((byte)(confirmSequenceId >> 24));
                st.WriteByte((byte)(confirmSequenceId >> 16));
                st.WriteByte((byte)(confirmSequenceId >> 8));
                st.WriteByte((byte)confirmSequenceId);

                m_Req.WriteTo(st);

                OnSent();
            }
            catch (Exception e)
            {
                Log.Error($"Rpc:{GetType()} encode failed. packetId:{PacketId} exception:{e}");
                return false;
            }

            return true;
        }

        public override void OnReceive()
        {
            if (m_State == RpcState.Wait)
            {
                // todo UI hide NetWaiting
                m_State = RpcState.Done;
                StopTimer();
                NetModule.RemoveSentRpc(this);

                // todo ErrorCode.Success
                if (m_ErrorCode == 0)
                {
                    Process();
                    // todo event
                }
                else
                {
                    Log.Debug($"Rpc:{GetType()} received. errorCode:{m_ErrorCode} packetId:{PacketId}");
                    if (!OnErrorCode(m_ErrorCode))
                    {
                        // todo event trigger
                    }
                }
            }
        }

        protected override void Process()
        {
            throw new NotImplementedException($"Rpc:{GetType()} not implemented Process");
        }

        public override void OnSend()
        {
        }

        public override string GetLogMsg()
        {
            return $"Rpc:{GetType()} packetId:{PacketId} sequenceId:{SequenceId} " +
                   $"timeoutMs:{TimeoutMs} req:{m_Req} resp:{m_Rsp}";
        }
    }
}