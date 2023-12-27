using System;
using System.Collections.Generic;
using System.Reflection;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Rpc
{
    public class PacketDispatcher
    {
        public class PacketHandler
        {
            public PacketBase Packet { get; private set; }

            public PacketHandler(PacketBase packet)
            {
                Packet = packet;
            }
        }

        public static readonly PacketDispatcher Instance = new();
        private readonly Dictionary<int, PacketHandler> m_PacketHandlers = new();

        public PacketHandler GetPacketHandler(int packetId)
        {
            m_PacketHandlers.TryGetValue(packetId, out PacketHandler handler);
            return handler;
        }

        private void RegisterPacket(int packetId, PacketBase packet)
        {
            var handler = new PacketHandler(packet);
            if (m_PacketHandlers.TryAdd(packetId, handler) == false)
            {
                Log.Error($"register repeated packet {packet.GetLogMsg()}");
            }
        }

        public void Init()
        {
            foreach (Type type in Utility.Assembly.GetTargetTypes(typeof(PacketBase)))
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(PacketBase)))
                {
                    var success = false;
                    foreach (ProtocolRegisterAttribute attr in type.GetCustomAttributes(
                                 typeof(ProtocolRegisterAttribute), false))
                    {
                        // 从当前类往上查找 ProtocolId 字段, 并赋值
                        Type baseType = type;
                        while (true)
                        {
                            FieldInfo fieldInfo =
                                baseType.GetField("ProtocolId", BindingFlags.Static | BindingFlags.Public);
                            if (fieldInfo == null)
                            {
                                baseType = baseType.BaseType;
                                if (baseType == null)
                                {
                                    throw new Exception($"{type} ProtocolId field does not exist");
                                }
                            }
                            else if (Activator.CreateInstance(type) is PacketBase packet)
                            {
                                fieldInfo.SetValue(null, attr.ProtocolId);
                                RegisterPacket(attr.ProtocolId, packet);

                                Log.Debug($"register packet {packet.GetLogMsg()}");

                                success = true;
                                break;
                            }
                        }
                    }

                    if (success == false)
                    {
                        Log.Error($"{type} ProtocolRegisterAttribute does not exist");
                        return;
                    }
                    
                    foreach (RpcTimeoutAttribute attr in type.GetCustomAttributes(typeof(RpcTimeoutAttribute), false))
                    {
                        // 从当前类往上查找 TimeoutMs, 并赋值
                        Type baseType = type;
                        while (true)
                        {
                            FieldInfo fieldInfo =
                                baseType.GetField("TimeoutMs", BindingFlags.Static | BindingFlags.Public);
                            if (fieldInfo == null)
                            {
                                baseType = baseType.BaseType;
                                if (baseType == null)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                fieldInfo.SetValue(null, attr.TimeoutMs);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}