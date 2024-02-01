using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    /// <summary>
    /// tcp连接类, 对Socket对象的封装, 提供读、写、关闭操作
    /// 1. 同一个时间只会投递一个Receive请求，所以OnReceived是单线程的
    /// 2. Send接口可以多线程调用
    /// 3. Close是低频操作，可以加锁操作
    /// </summary>
    public class TcpConnection
    {
        private static readonly OperationPool<SocketAsyncEventArgs> m_SaeaPool = new(CreateSaea, CleanSaea, 8);

        private static SocketAsyncEventArgs CreateSaea()
        {
            return new SocketAsyncEventArgs();
        }

        private static void CleanSaea(SocketAsyncEventArgs args)
        {
            args.UserToken = null;
        }

        private Socket m_Socket;
        private ITcpHandler m_Handler;
        private IOBuffer m_RecvBuf;
        private readonly SocketAsyncEventArgs m_RecvArgs;
        private int m_SendBufLen;
        private int m_MaxSendBufLen;
        private int m_Closed;

        public IPEndPoint RemoteEndPoint { get; private set; }

        // 自定义数据
        public object CustomObject { get; set; }

        // 连接状态
        public ConnectionState ConnState { get; private set; }

        public TcpConnection(ITcpHandler handler = null)
        {
            ConnState = ConnectionState.Idle;
            m_Handler = handler;
            m_RecvArgs = new SocketAsyncEventArgs();
            m_RecvArgs.Completed += OnReceiveComplete;
        }

        public void Init(ITcpHandler handler, int recvBufLen, int maxSendBufLen = 0)
        {
            m_Handler = handler;
            m_RecvBuf = new IOBuffer(recvBufLen);
            m_MaxSendBufLen = maxSendBufLen;
        }

        private void BeginRecv()
        {
            if (ConnState != ConnectionState.Connected)
            {
                return;
            }

            m_RecvArgs.SetBuffer(m_RecvBuf.Data, m_RecvBuf.Tail, m_RecvBuf.FreeLength);
            try
            {
                // todo 递归, 存在栈溢出风险?
                if (!m_Socket.ReceiveAsync(m_RecvArgs))
                {
                    OnReceiveComplete(m_Socket, m_RecvArgs);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Begin recv failed. exception:{e}");
                Close();
            }
        }

        private void OnReceiveComplete(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    var bytesTransferred = args.BytesTransferred;
                    if (bytesTransferred == 0)
                    {
                        Log.Error("On recv complete failed, conn closed.");
                        Close();
                    }
                    else
                    {
                        m_RecvBuf.MoveTail(bytesTransferred);
                        m_Handler.NetThreadOnReceive(this, m_RecvBuf);
                        if (m_RecvBuf.FreeLength == 0)
                        {
                            m_RecvBuf.AlignToHead();
                            if (m_RecvBuf.FreeLength == 0)
                            {
                                Close();
                                return;
                            }
                        }

                        BeginRecv();
                    }
                }
                else
                {
                    Log.Error($"On recv complete failed. socketError:{args.SocketError}");
                    Close();
                }
            }
            catch (Exception e)
            {
                Log.Error($"On recv complete failed. exception:{e}");
                Close();
            }
        }

        public bool Send(byte[] buffer, int offset, int length, Action<byte[]> callback = null)
        {
            if (ConnState != ConnectionState.Connected)
            {
                return false;
            }

            Interlocked.Add(ref m_SendBufLen, length);
            if (m_MaxSendBufLen > 0 && m_SendBufLen > m_MaxSendBufLen)
            {
                Close(true);
                Log.Error("Send failed, over MaxSendBufLen");
                return false;
            }

            SocketAsyncEventArgs args = m_SaeaPool.Take();
            args.Completed += OnSendComplete;
            args.UserToken = callback;
            args.SetBuffer(buffer, offset, length);

            try
            {
                if (!m_Socket.SendAsync(args))
                {
                    OnSendComplete(m_Socket, args);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Send failed. exception:{e}");
                Close();
                return false;
            }

            return true;
        }

        private void OnSendComplete(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    Interlocked.Add(ref m_SendBufLen, -args.BytesTransferred);
                }
                else
                {
                    Log.Error($"On send complete failed. socketError:{args.SocketError}");
                    Close();
                }
            }
            catch (Exception e)
            {
                Log.Error($"On send complete failed. exception:{e}");
                Close();
            }

            if (args.UserToken is Action<byte[]> cb)
            {
                cb(args.Buffer);
            }

            args.SetBuffer(null, 0, 0);
            args.Completed -= OnSendComplete;
            args.UserToken = null;

            m_SaeaPool.Add(args);
        }

        public void Close(bool force = true)
        {
            if (Interlocked.Exchange(ref m_Closed, 1) == 0)
            {
                m_Socket.Close();
                if (ConnState == ConnectionState.Connected)
                {
                    m_Handler.NetThreadOnClose(this, SocketError.Shutdown);
                }
                else if (ConnState == ConnectionState.Connecting)
                {
                    m_Handler.NetThreadOnConnectFailed(this, SocketError.TimedOut);
                }

                ConnState = ConnectionState.Closed;
            }
        }

        private bool IsClosed()
        {
            return ConnState == ConnectionState.Closed;
        }

        public bool Connect(IPEndPoint ep)
        {
            m_Socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            var args = new SocketAsyncEventArgs();
            args.Completed += ConnectCompleted;
            args.RemoteEndPoint = ep;
            RemoteEndPoint = ep;
            ConnState = ConnectionState.Connecting;

            if (!m_Socket.ConnectAsync(args))
            {
                // 连接已完成，直接处理结果
                ConnectCompleted(m_Socket, args);
            }

            return true;
        }

        private void ConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && m_Socket.Connected)
            {
                args.UserToken = null;
                args.RemoteEndPoint = null;

                ConnState = ConnectionState.Connected;

                m_Handler.NetThreadOnConnect(this);

                if (!IsClosed())
                {
                    BeginRecv();
                }
            }
            else
            {
                ConnState = ConnectionState.Idle;
                m_Handler.NetThreadOnConnectFailed(this, args.SocketError);
            }
        }
    }
}