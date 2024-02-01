### IOBuffer

1. 用于单个连接的消息接收 `TcpConnection.m_RecvBuf`
2. 单个连接上的消息接收内存分配, 减少GC
3. 本质上是一个预分配固定大小的数组内存

### ClientBuffer

1. 用于发送消息 `ClientNet.m_SendBuffer`
2. 本质上是一个 `struct Buffer` 数组
3. 客户端缓存指定数量的已发送包数据(回包后游标会移动),
   当客户端发生重连时, 会将未收到回包的缓存包重新发送

### 一个连接包含的实例

* 一个 `GateNet extends ClientNetBase` 对象
    * 一个 `ClientBuffer` 对象
    * 一个 `Session` 对象
        * 一个 `TcpConnection` 对象
            * 一个 `IOBuffer` 对象
            * 一个 `Socket` 对象
        * 一个 `KcpConnection` 对象

### Tcp/Kcp 双通道思考

1. 双通道通信仅建立于 client 和 gate 之间
2. 内部服务节点之间仅建立 Tcp 连接
3. client 通过 rpc_heartbeat(心跳包) 超时切换 kcp 通道
   1. rpc 会有超时机制, ptc没有超时机制, 所以心跳包通过rpc发送
   2. rpc_heartbeat 一直通过tcp发送, 检测到网络恢复时切回tcp ???
   3. 看一下重连的依据标准, 是否对重连有影响 ???
   4. tcp/kcp 通道切换时, 如何确保不重复发包或少发包 ???
   5. 弱网环境, tcp的心跳包一直发送不成功, 连接被服务器终端怎么解决 ???
      1. 弱网环境的心跳包应该由kcp发送, tcp的心跳包也要继续发, 当tcp心跳包检测到网络恢复后切换成tcp通道
4. Kcp 消息分为两种:
   1. 某个业务主动选择Kcp通道发送消息
      1. 实现方案: PtcPacket/RpcPacket 扩展子类, PacketType 类型增加Kcp类型
   2. 当处于弱网环境时, 底层主动切换为Kcp通道发送消息

### 一个连接建立过程
1. 