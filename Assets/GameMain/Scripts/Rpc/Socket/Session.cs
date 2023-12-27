using System.Net;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public sealed class Session
    {
        public IPEndPoint EndPoint { get; set; }
        public TcpConnection Conn { get; private set; }
        public NetState State { get; set; }
        public int Timeout { get; set; }
        public int ReconnectNum { get; set; }
        public int ConnectedNum { get; set; }
        public INetEventHandler NetEventHandler { get; set; }
        public ITcpHandler TcpHandler { get; set; }
        public TimerTask Timer { get; set; }

        public Session(INetEventHandler netEventHandler)
        {
            NetEventHandler = netEventHandler;
        }

        private void StartTimer()
        {
            StopTimer();
            Timer = TimerManager.Instance.AddTimer(Timeout, OnConnectTimeout, -1);
        }

        private void StopTimer()
        {
            Timer?.Cancel();
            Timer = null;
        }

        private void OnConnectTimeout()
        {
            if (State != NetState.Connected)
            {
                Log.Error($"session connect timeout. ep:{EndPoint} reconnectNum:{ReconnectNum}");
                State = NetState.DisConnect;
                NetEventHandler.OnConnectFailed(ReconnectNum);

                if (State != NetState.ForceClose)
                {
                    Connect();
                    ReconnectNum++;
                }
            }
        }

        public void Connect()
        {
            // 关闭旧连接
            Close();

            // todo ui ShowNetWaiting

            // 建立新连接
            Conn = new TcpConnection();
            Conn.Init(TcpHandler, 1 * 1024 * 1024);
            State = NetState.Connecting;
            Conn.Connect(EndPoint);

            Log.Info("start connecting {0}", EndPoint);
            StartTimer();
        }

        public void Close()
        {
            // TODO ui HideNetWaiting

            Conn?.Close();
            Conn = null;
            StopTimer();
        }
    }
}