using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public class LoginNet : ClientNetBase, INetEventHandler
    {
        public static readonly LoginNet Instance = new();

        public LoginNet()
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
            if (reconnectNum >= 3)
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