using System;
using System.Net;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public sealed class Session
    {
        public IPEndPoint TcpEndPoint { get; set; }
        public IPEndPoint KcpEndPoint { get; set; }
        public TcpConnection TcpConn { get; private set; }

        // public KcpConnection KcpConn { get; private set; }
        // public int KcpListenPort { get; set; }
        public NetState State { get; set; }
        public int Timeout { get; set; }
        public int ReconnectNum { get; set; }
        public int ConnectedNum { get; set; }
        public ITcpHandler TcpHandler { get; set; }

        private readonly INetEventHandler m_NetEventHandler;
        private TimerTask m_TcpTimer;

        // private TimerTask m_KcpTimer;

        /// <summary>
        /// 网络是否稳定
        /// </summary>
        // private bool m_NetStability = true;

        public Session(INetEventHandler netEventHandler)
        {
            m_NetEventHandler = netEventHandler;
        }

        private void StartTimer()
        {
            StopTimer();
            m_TcpTimer = TimerManager.Instance.AddTimer(Timeout, OnConnectTimeout, -1);
        }

        private void StopTimer()
        {
            m_TcpTimer?.Cancel();
            m_TcpTimer = null;
        }

        private void OnConnectTimeout()
        {
            if (State != NetState.Connected)
            {
                Log.Error($"Session connect timeout. ep:{TcpEndPoint}");
                State = NetState.DisConnect;
                m_NetEventHandler.OnConnectFailed(ReconnectNum);

                if (State != NetState.ForceClose)
                {
                    ReconnectNum++;
                    TcpConnect();
                }
            }
        }

        public void TcpConnect()
        {
            // 关闭旧连接
            Close();

            // todo ui ShowNetWaiting

            // 建立新连接
            TcpConn = new TcpConnection();
            TcpConn.Init(TcpHandler, 1 * 1024 * 1024);
            State = NetState.Connecting;
            TcpConn.Connect(TcpEndPoint);

            Log.Info($"Start connecting {TcpEndPoint}. reconnectNum:{ReconnectNum}");
            StartTimer();
        }

        // // todo kcp connect
        // public void KcpConnect()
        // {
        // // 关闭旧连接
        // KcpConn?.Close();
        // // 建立 kcp 新连接
        // KcpConn = new KcpConnection();
        // // KcpConn.Init();
        // KcpConn.Connect(KcpListenPort, KcpEndPoint);
        //
        // Log.Info($"Start connecting {KcpEndPoint}");
        // StartKcpTimer();
        // }

        // private void StartKcpTimer()
        // {
        //     StopKcpTimer();
        //     m_KcpTimer = TimerManager.Instance.AddTimer(10, OnUpdateKcp, -1);
        // }

        // private void OnUpdateKcp()
        // {
        //     KcpConn.Kcp.Update(DateTimeOffset.UtcNow);
        // }

        // private void StopKcpTimer()
        // {
        //     m_KcpTimer?.Cancel();
        //     m_KcpTimer = null;
        // }

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