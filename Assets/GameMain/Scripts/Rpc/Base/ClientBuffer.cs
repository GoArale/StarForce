using System;

namespace GameMain.Rpc
{
    public class ClientBuffer
    {
        private struct Buffer
        {
            public byte[] m_Data;
            public int m_Length;
            public uint m_Sequence;
            public Action<byte[]> m_CB;
        }

        private readonly Buffer[] m_Buffer;
        private int m_Header;
        private int m_Tail;

        public ClientBuffer(int length)
        {
            m_Buffer = new Buffer[length];
        }

        public void Push(uint sequence, byte[] data, int length, Action<byte[]> cb = null)
        {
            var buffer = new Buffer() { m_Data = data, m_Length = length, m_Sequence = sequence, m_CB = cb };
            m_Buffer[m_Tail] = buffer;

            m_Tail = (m_Tail + 1) % m_Buffer.Length;
            if (m_Tail == m_Header)
            {
                m_Header = (m_Header + 1) % m_Buffer.Length;
            }
        }

        // 这里有一定安全问题， 客户端乱发确认sequence， 服务器还没发送完， 则可能导致内存复用出现问题
        public void ConfirmSequence(uint sequence)
        {
            while (m_Header != m_Tail)
            {
                Buffer b = m_Buffer[m_Header];
                if (b.m_Sequence <= sequence)
                {
                    b.m_CB?.Invoke(b.m_Data);

                    b.m_CB = null;
                    b.m_Data = null;
                }
                else
                {
                    break;
                }

                m_Header = (m_Header + 1) % m_Buffer.Length;
            }
        }

        public void ResendData(uint sequence, TcpConnection conn)
        {
            ConfirmSequence(sequence);
            
            var header = m_Header;
            while (header != m_Tail)
            {
                Buffer b = m_Buffer[header];
                conn.Send(b.m_Data, 0, b.m_Length);
                header = (header + 1) % m_Buffer.Length;
            }
        }
    }
}