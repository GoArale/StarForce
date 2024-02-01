using KP2Protocol;
using GameMain.Rpc;

/* Modify Time: 2024/2/1 17:07:47 */

namespace GameMain
{
    [ProtocolRegister(1317)]
    public partial class Ptc_Gate2Client_LoginOtherwhere : PtcPacket<Ptc_Gate2Client_LoginOtherwhere, PtcLoginOtherwhereData> {}

    [ProtocolRegister(370)]
    public partial class Ptc_Game2Client_RoleReconnectNotify : PtcPacket<Ptc_Game2Client_RoleReconnectNotify, PtcRoleReconnectNotifyData> {}

    [ProtocolRegister(2189)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Gate_ClientReconnect : RpcPacket<Rpc_Client2Gate_ClientReconnect, RpcClientReconnectReq, RpcClientReconnectRsp> {}
    [ProtocolRegister(2796)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Gate_EnterZone : RpcPacket<Rpc_Client2Gate_EnterZone, RpcEnterZoneReq, RpcEnterZoneRsp> {}
    [ProtocolRegister(1383)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Balance_Login : RpcPacket<Rpc_Client2Balance_Login, RpcLoginReq, RpcLoginRsp> {}
}
