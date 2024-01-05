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