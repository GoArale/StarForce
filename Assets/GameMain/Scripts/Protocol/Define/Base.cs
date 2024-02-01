using KP2Protocol;
using GameMain.Rpc;

/* Modify Time: 2024/2/1 20:40:26 */

namespace GameMain
{
    [ProtocolRegister(9569)]
    public partial class Ptc_Game2Client_NotifyAvatarOpen : PtcPacket<Ptc_Game2Client_NotifyAvatarOpen, PtcNotifyAvatarOpenData> {}

    [ProtocolRegister(2634)]
    public partial class Ptc_Game2Client_NotifySystemOpen : PtcPacket<Ptc_Game2Client_NotifySystemOpen, PtcNotifySystemOpenData> {}

    [ProtocolRegister(9325)]
    public partial class Ptc_Client2Game_OpenSystemUI : PtcPacket<Ptc_Client2Game_OpenSystemUI, PtcOpenSystemUIData> {}

    [ProtocolRegister(2067)]
    public partial class Ptc_Game2Client_PassDayNotify : PtcPacket<Ptc_Game2Client_PassDayNotify, PtcPassDayNotifyData> {}

    [ProtocolRegister(5325)]
    public partial class Ptc_Client2Game_SyncTimeC2S : PtcPacket<Ptc_Client2Game_SyncTimeC2S, PtcSyncTimeC2SData> {}

    [ProtocolRegister(2741)]
    public partial class Ptc_Game2Client_SyncTimeS2C : PtcPacket<Ptc_Game2Client_SyncTimeS2C, PtcSyncTimeS2CData> {}

    [ProtocolRegister(3836)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Game_Heartbeat : RpcPacket<Rpc_Client2Game_Heartbeat, RpcHeartbeatReq, RpcHeartbeatRsp> {}
    [ProtocolRegister(9333)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Game_SetAvatarHeroID : RpcPacket<Rpc_Client2Game_SetAvatarHeroID, RpcSetAvatarHeroIDReq, RpcSetAvatarHeroIDRsp> {}
    [ProtocolRegister(4220)]
    [RpcTimeout(5000)]
    public partial class Rpc_Client2Game_SetRoleSetting : RpcPacket<Rpc_Client2Game_SetRoleSetting, RpcSetRoleSettingReq, RpcSetRoleSettingRsp> {}
}
