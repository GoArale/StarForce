using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.IO;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public abstract partial class ClientNetBase : ITcpHandler
    {
        public void NetThreadOnConnect(TcpConnection conn)
        {
            OneThreadSyncContext.Instance.Post(() => { OnConnect(conn); });
        }

        public void NetThreadOnConnectFailed(TcpConnection conn, SocketError error)
        {
            OneThreadSyncContext.Instance.Post(() => { OnConnectFailed(conn, error); });
        }

        public void NetThreadOnReceive(TcpConnection conn, IOBuffer buffer)
        {
            while (true)
            {
                if (buffer.Length < 4)
                {
                    return;
                }

                var pktLen = (buffer[0] << 24) + (buffer[1] << 16) + (buffer[2] << 8) + buffer[3];

                if (buffer.Length < pktLen)
                {
                    return;
                }

                // 包长限制
                if (pktLen < 17)
                {
                    conn.Close();
                    return;
                }

                var sequenceId = ((uint)buffer[4] << 24) + ((uint)buffer[5] << 16) +
                                 ((uint)buffer[6] << 8) + buffer[7];

                m_LastRecvSequenceId = sequenceId;

                var pt = buffer[8];
                if (!Enum.IsDefined(typeof(PacketType), pt))
                {
                    conn.Close();
                    return;
                }

                var pktType = (PacketType)pt;
                var pktId = (buffer[9] << 24) + (buffer[10] << 16) + (buffer[11] << 8) + buffer[12];
                var confirmSequenceId = ((uint)buffer[13] << 24) + ((uint)buffer[14] << 16) +
                                        ((uint)buffer[15] << 8) + buffer[16];
                var pos = 17;

                PacketBase pkt;
                if (pktType == PacketType.RpcResp)
                {
                    var rpcSequenceId = ((uint)buffer[pos] << 24) + ((uint)buffer[pos + 1] << 16) +
                                        ((uint)buffer[pos + 2] << 8) + buffer[pos + 3];
                    pos += 4;

                    RpcBase rpcPkt = GetSentRpcPacket(rpcSequenceId);
                    if (rpcPkt == null)
                    {
                        // todo 超时的 rpc, 打个 warning 日志
                        buffer.MoveHead(pktLen);
                        continue;
                    }

                    rpcPkt.NetModule = this;
                    pkt = rpcPkt;
                }
                else
                {
                    PacketDispatcher.PacketHandler handler = PacketDispatcher.Instance.GetPacketHandler(pktId);
                    if (handler == null)
                    {
                        // todo 不能处理的协议， 可能是服务器更新了新的协议， 老的客户端需要忽略
                        buffer.MoveHead(pktLen);
                        continue;
                    }

                    pkt = handler.Packet.Create();
                }

                // setting client sessionId
                pkt.SequenceId = sequenceId;
                if (!pkt.Decode(buffer.Data, buffer.Head + pos, pktLen - pos))
                {
                    // 包解析出错
                    conn.Close();
                    return;
                }

                OneThreadSyncContext.Instance.Post(() =>
                {
                    m_SendBuffer.ConfirmSequence(confirmSequenceId);
                    pkt.OnReceive();
                });

                buffer.MoveHead(pktLen);
            }
        }

        public void NetThreadOnClose(TcpConnection conn, SocketError error)
        {
            OneThreadSyncContext.Instance.Post(() => { OnClose(conn, error); });
        }

        private bool IsConnected()
        {
            return m_Session.Conn != null && m_Session.Conn.ConnState == ConnectionState.Connected;
        }

        private bool DoSendPacket(PacketBase packet, bool cache = true)
        {
            if (!cache && !IsConnected())
            {
                return false;
            }

            RecyclableMemoryStream st = m_Stream;
            st.SetLength(0);
            st.Seek(4, SeekOrigin.Begin);

            // 1. sequenceId
            packet.SequenceId = NextSequenceId();

            // 2. data
            if (!packet.Encode(st, m_LastRecvSequenceId))
            {
                return false;
            }

            // 3. pkgLen
            var pktLen = st.Length;
            st.Seek(0, SeekOrigin.Begin);
            st.WriteByte((byte)(pktLen >> 24));
            st.WriteByte((byte)(pktLen >> 16));
            st.WriteByte((byte)(pktLen >> 8));
            st.WriteByte((byte)(pktLen));

            // send
            packet.OnSend();
            var bytes = st.GetBuffer();
            if (cache)
            {
                m_SendBuffer.Push(packet.SequenceId, bytes, (int)pktLen);
                if (IsConnected())
                {
                    return m_Session.Conn.Send(bytes, 0, (int)pktLen);
                }

                return false;
            }

            return m_Session.Conn.Send(bytes, 0, (int)pktLen);
        }

        private uint NextSequenceId()
        {
            return ++m_LastSendSequenceId;
        }

        public bool AddSentRpc(RpcBase packet)
        {
            return m_SentRpcDic.TryAdd(packet.SequenceId, packet);
        }

        public void RemoveSentRpc(RpcBase packet)
        {
            m_SentRpcDic.TryRemove(packet.SequenceId, out _);
        }

        private RpcBase GetSentRpcPacket(uint sequenceId)
        {
            m_SentRpcDic.TryGetValue(sequenceId, out RpcBase packet);
            return packet;
        }
    }
}