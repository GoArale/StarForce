namespace GameMain.Rpc
{
    public interface INetEventHandler
    {
        void OnConnect();
        void OnReconnect();
        void OnConnectFailed(int reconnectNum);
        void OnDisconnect();
    }
}