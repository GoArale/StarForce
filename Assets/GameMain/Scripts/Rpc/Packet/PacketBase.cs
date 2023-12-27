using System.IO;
using GameFramework;
using GameFramework.ObjectPool;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public abstract class PacketBase : ObjectBase
    {
        // message id
        public uint SequenceId { get; set; }
        public abstract int PacketId { get; }

        public abstract PacketBase Create();
        public abstract bool Decode(byte[] data, int offset, int length);
        public abstract bool Encode(MemoryStream st, uint confirmSequenceId);
        protected abstract void Process();
        public abstract void OnReceive();
        public abstract void OnSend();
        public abstract string GetLogMsg();
    }

    public abstract class PtcBase : PacketBase
    {
    }

    public abstract class RpcBase : PacketBase
    {
        private TimerTask m_Timer;
        protected int m_State = RpcState.Wait;

        public ClientNetBase NetModule { get; set; }
        protected abstract int Timeout { get; }

        /// <summary>
        /// 已发送
        /// </summary>
        protected void OnSent()
        {
            m_State = RpcState.Wait;

            // todo UI show NetWaiting

            if (!NetModule.AddSentRpc(this))
            {
                Log.Error($"Rpc:{GetType()} add SentDictionary failed. sequenceId:{SequenceId}");
            }

            if (GameEntry.GameGlobal.EnableRpcTimeout)
            {
                m_Timer = TimerManager.Instance.AddTimer(Timeout, OnResponseTimeout);
            }
        }

        private void OnResponseTimeout()
        {
            if (m_State == RpcState.Wait)
            {
                m_State = RpcState.Timeout;
                // todo UI hide NetWaiting()
                OnTimeout();
                NetModule.RemoveSentRpc(this);
            }
        }

        protected void StopTimer()
        {
            m_Timer?.Cancel();
            m_Timer = null;
        }

        /// <summary>
        /// 请求超时
        /// </summary>
        protected virtual void OnTimeout()
        {
            // todo errorCode 服务器繁忙, 请稍后重试
            Log.Error($"Rpc:{GetType()} response timeout. sequenceId:{SequenceId}");
        }

        // /// <summary>
        // /// 请求应答
        // /// </summary>
        // protected virtual void OnReply()
        // {
        //     throw new NotImplementedException($"$Rpc:{GetType()} not implement OnRelay");
        // }

        /// <summary>
        /// 错误码处理
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnErrorCode(int errorCode)
        {
            // todo protocol errorCode
            return false;
        }
    }
}