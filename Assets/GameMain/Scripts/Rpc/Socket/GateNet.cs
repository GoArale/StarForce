using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public class GateNet : ClientNetBase, INetEventHandler
    {
        public static GateNet Instance { get; } = new();

        private GateNet()
        {
            m_NetEventHandler = this;
            // Rpc_Client2Gate_ClientReconnect.OnMsg.Add((req, rsp) =>{
            //     m_sendBuffer.ReSendData(rsp.LastRecvSequence, m_Session.m_conn);
            //     Log.Info($"Resend Data from Sequence: {rsp.LastRecvSequence}");
            // });
        }

        public void OnConnect()
        {
        }

        public void OnReconnect()
        {
            // todo Rpc_Client2Gate_ClientReconnect
        }

        public void OnConnectFailed(int reconnectNum)
        {
            if (reconnectNum >= GameEntry.GameGlobal.ReconnectNum)
            {
                CloseConnection();
                // todo event
            }
        }

        public void OnDisconnect()
        {
            Log.Warning("GateNet disconnect");
        }
    }
}