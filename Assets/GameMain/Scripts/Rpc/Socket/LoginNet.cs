using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public class LoginNet : ClientNetBase, INetEventHandler
    {
        public static LoginNet Instance { get; } = new();

        private LoginNet()
        {
            m_NetEventHandler = this;
        }

        public void OnConnect()
        {
        }

        public void OnReconnect()
        {
        }

        public void OnConnectFailed(int reconnectNum)
        {
            if (reconnectNum >= GameEntry.GameGlobal.ReconnectNum)
            {
                CloseConnection();
                // todo disconnect event
            }
        }

        public void OnDisconnect()
        {
            Log.Warning("LoginNet connection disconnect");
        }
    }
}