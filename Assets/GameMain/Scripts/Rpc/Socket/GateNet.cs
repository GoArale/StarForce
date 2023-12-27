using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public class GateNet : ClientNetBase, INetEventHandler
    {
        // public static readonly GateNetComponent Instance = new();

        public GateNet()
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
        }

        public void OnDisconnect()
        {
            Log.Warning("GateNet disconnect");
        }
    }
}