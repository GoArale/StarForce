using System;
using System.Collections.Generic;
using System.Reflection;
using GameFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.RpcNetwork
{
    public class PacketDispatcher
    {
        public class PacketHandler
        {
            private PacketBase m_Packet;

            public PacketHandler(PacketBase packet)
            {
                m_Packet = packet;
            }
        }

        public static readonly PacketDispatcher Instance = new();
        private readonly Dictionary<int, PacketHandler> m_PacketHandlers = new();

        public PacketHandler GetPacketHandler(int packetId)
        {
            m_PacketHandlers.TryGetValue(packetId, out PacketHandler handler);
            return handler;
        }

        private void RegisterProtocol(int packetId, PacketBase packet)
        {
            var handler = new PacketHandler(packet);
            if (m_PacketHandlers.TryAdd(packetId, handler) == false)
            {
                Log.Error($"register protocol is repeated. packet:{packet.GetType()} packetId:{packetId}");
            }
        }
        
        public void Init()
        {
            foreach (Type type in Utility.Assembly.GetTargetTypes(typeof(PacketBase)))
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(PacketBase)))
                {
                    var success = false;
                    foreach (PacketRegisterAttribute packetAttr in type.GetCustomAttributes(typeof(PacketRegisterAttribute), false))
                    {
                        // 从当前类往上查找 PacketId 字段, 并赋值
                        Type baseType = type;
                        while (true)
                        {
                            FieldInfo packetIdFiled = type.GetField("PacketId", BindingFlags.Public);
                            if (packetIdFiled == null)
                            {
                                baseType = baseType.BaseType;
                                if (baseType == null)
                                {
                                    throw new Exception($"{type} PacketId field does not exist");
                                }
                            }
                            else
                            {
                                packetIdFiled.SetValue(null, packetAttr.PacketId);
                                RegisterProtocol(packetAttr.PacketId, Activator.CreateInstance(type) as PacketBase);

                                success = true;
                                break;
                            }
                        }
                    }

                    if (success == false)
                    {
                        Log.Error($"{type} PacketRegisterAttribute does not exist");
                        return;
                    }

                    foreach (RpcTimeoutAttribute attr in type.GetCustomAttributes(typeof(RpcTimeoutAttribute), false))
                    {
                        // 从当前类往上查找 TimeoutMS, 并赋值
                        Type baseType = type;
                        while (true)
                        {
                            FieldInfo timeoutField = type.GetField("TimeoutMS", BindingFlags.Public);
                            if (timeoutField == null)
                            {
                                baseType = baseType.BaseType;
                                if (baseType == null)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                timeoutField.SetValue(null, attr.TimeoutMS);
                                break;
                            }
                        }
                    }
                }
            }
        } 
    }
}