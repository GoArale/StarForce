using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Microsoft.IO;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public abstract partial class ClientNetBase
    {
        private static readonly RecyclableMemoryStream m_Stream = new(new RecyclableMemoryStreamManager());
        private readonly ConcurrentDictionary<uint, RpcBase> m_SentRpcDic = new();

        protected INetEventHandler m_NetEventHandler;
        private readonly ClientBuffer m_SendBuffer = new(128);
        private Session m_Session;

        // 上个连接最后收到的序列号, 用于断线重连告诉服务器重新下发丢掉的包
        private uint m_LastRecvSequenceId;
        private uint m_LastSendSequenceId;

        public void Connect(string ip, int port, int timeoutMs = 3000)
        {
            if (m_Session == null)
            {
                m_Session = new Session(m_NetEventHandler);
            }
           
            m_Session.EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            m_Session.Timeout = timeoutMs;
            m_Session.TcpHandler = this;
            
            m_Session.Connect();
            m_Session.ReconnectNum = 0;
            m_Session.ConnectedNum = 0;
        }

        public bool SendPacket(RpcBase packet, bool cache = true)
        {
            if (m_Session == null) return false;
            packet.NetModule = this;
            return DoSendPacket(packet, cache);
        }

        public bool SendPacket(PtcBase packet, bool cache = true)
        {
            return m_Session != null && DoSendPacket(packet, cache);
        }

        public void CloseConnection()
        {
            if (m_Session == null) return;

            m_Session.State = NetState.ForceClose;
            m_Session.Close();
            m_Session = null;
        }

        public NetState GetNetState()
        {
            if (m_Session == null) return NetState.DisConnect;

            return m_Session.State;
        }

        private void OnClose(TcpConnection conn, SocketError error)
        {
            if (m_Session != null && m_Session.TcpConn == conn)
            {
                if (m_Session.State == NetState.Connected)
                {
                    m_Session.State = NetState.DisConnect;
                    m_NetEventHandler.OnDisconnect();
                }
                else if (m_Session.State == NetState.Connecting)
                {
                    m_Session.State = NetState.DisConnect;
                    m_NetEventHandler.OnConnectFailed(m_Session.ReconnectNum);
                }
            }
        }

        private void OnConnect(TcpConnection conn)
        {
            if (m_Session != null && m_Session.TcpConn == conn)
            {
                // todo ui HideNetWaiting

                Log.Info($"Connect to {conn.RemoteEndPoint} succeed");

                m_Session.State = NetState.Connected;
                m_Session.ReconnectNum = 0;

                if (m_Session.ConnectedNum == 0)
                {
                    m_NetEventHandler.OnConnect();
                }
                else
                {
                    m_NetEventHandler.OnReconnect();
                }

                m_Session.ConnectedNum++;
            }
        }

        private void OnConnectFailed(TcpConnection conn, SocketError error)
        {
            if (m_Session != null && m_Session.TcpConn == conn)
            {
                Log.Error($"Connect to {conn.RemoteEndPoint} failed. reason:{error}");
                if (m_Session.State != NetState.DisConnect)
                {
                    m_Session.State = NetState.DisConnect;
                    m_NetEventHandler.OnConnectFailed(m_Session.ReconnectNum);
                }
            }
        }
    }
}