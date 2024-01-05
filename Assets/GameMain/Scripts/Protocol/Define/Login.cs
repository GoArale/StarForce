using KP2Protocol;
using GameMain.Rpc;

/* Modify Time: 2024/1/19 20:40:59 */

namespace GameMain
{
    [ProtocolRegister(2189)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Gate_ClientReconnect : RpcPacket<Rpc_Client2Gate_ClientReconnect,
        RpcClientReconnectReq, RpcClientReconnectRsp>
    {
    }

    [ProtocolRegister(2796)]
    [RpcTimeout(5000)]
    public partial class
        Rpc_Client2Gate_EnterZone : RpcPacket<Rpc_Client2Gate_EnterZone, RpcEnterZoneReq, RpcEnterZoneRsp>
    {
    }

    [ProtocolRegister(1383)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Balance_Login : RpcPacket<Rpc_Client2Balance_Login, RpcLoginReq, RpcLoginRsp>
    {
    }

    [ProtocolRegister(3392)]
    [RpcTimeout(5000)]
    public partial class
        Rpc_Client2Balance_LoginTest : RpcPacket<Rpc_Client2Balance_LoginTest, RpcLoginReq, RpcLoginRsp>
    {
    }
}