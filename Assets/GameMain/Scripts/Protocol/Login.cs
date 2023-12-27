namespace GameMain.Rpc
{
    [ProtocolRegister(1383)]
    [RpcTimeout(5000)]
    public partial class
        RpcPacketClient2BalanceLogin : RpcPacket<RpcPacketClient2BalanceLogin, RpcLoginReq, RpcLoginRsp>
    {
    }
}