using System.Net.Sockets;

namespace GameMain.Rpc
{
    public interface ITcpHandler
    {
        void NetThreadOnConnect(TcpConnection conn);
        void NetThreadOnConnectFailed(TcpConnection conn, SocketError error);
        void NetThreadOnReceive(TcpConnection conn, IOBuffer buffer);
        void NetThreadOnClose(TcpConnection conn, SocketError error);
    }
}