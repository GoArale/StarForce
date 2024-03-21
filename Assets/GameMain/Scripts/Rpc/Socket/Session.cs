using System;
using System.Net;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public sealed class Session
    {
        public IPEndPoint EndPoint { get; set; }
        public TcpConnection TcpConn { get; private set; }
        public NetState State { get; set; }
        public int Timeout { get; set; }
        public int ReconnectNum { get; set; }
        public int ConnectedNum { get; set; }
        public ITcpHandler TcpHandler { get; set; }

        private readonly INetEventHandler m_NetEventHandler;
        private TimerTask m_Timer;

        /// <summary>
        /// 网络是否稳定
        /// </summary>
        private bool m_NetStability = true;

        public Session(INetEventHandler netEventHandler)
        {
            m_NetEventHandler = netEventHandler;
        }

        private void StartTimer()
        {
            StopTimer();
            m_Timer = TimerManager.Instance.AddTimer(Timeout, OnConnectTimeout, -1);
        }

        private void StopTimer()
        {
            m_Timer?.Cancel();
            m_Timer = null;
        }

        private void OnConnectTimeout()
        {
            if (State != NetState.Connected)
            {
                Log.Error($"Session connect timeout. ep:{EndPoint}");
                State = NetState.DisConnect;
                m_NetEventHandler.OnConnectFailed(ReconnectNum);

                if (State != NetState.ForceClose)
                {
                    ReconnectNum++;
                    Connect();
                }
            }
        }

        public void Connect()
        {
            // 关闭旧连接
            Close();

            // todo ui ShowNetWaiting

            // 建立新连接
            TcpConn = new TcpConnection();
            TcpConn.Init(TcpHandler, 1 * 1024 * 1024);
            State = NetState.Connecting;
            TcpConn.Connect(EndPoint);

            Log.Info($"Start connecting {EndPoint}. reconnectNum:{ReconnectNum}");
            StartTimer();
        }
        
        public void Close()
        {
            // TODO ui HideNetWaiting

            TcpConn?.Close();
            TcpConn = null;
            StopTimer();
        }

        // public bool Send(byte[] buffer, int offset, int length, Action<byte[]> callback = null)
        // {
        //     if (m_NetStability)
        //     {
        //         return TcpConn.Send(buffer, offset, length, callback);
        //     }
        //
        //     KcpConn.SendAsync(buffer, offset, length);
        //     return true;
        // }
    }
}