namespace GameMain.Rpc
{
    public interface INetEventHandler
    {
        /// <summary>
        /// 连接成功
        /// </summary>
        void OnConnect();

        /// <summary>
        /// 重连成功
        /// </summary>
        void OnReconnect();

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <param name="reconnectNum"></param>
        void OnConnectFailed(int reconnectNum);

        /// <summary>
        /// 连接断开
        /// </summary>
        void OnDisconnect();
    }
}