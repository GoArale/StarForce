using System;
using GameFramework;
using GameMain.Rpc;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain
{
    public class GameGlobalComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 开启 Rpc 超时检查
        /// </summary>
        [SerializeField] private bool m_EnableRpcTimeout = true;

        /// <summary>
        /// 网络重连次数
        /// </summary>
        [SerializeField] private int m_ReconnectNum = 3;

        /// <summary>
        /// Server IP Address
        /// </summary>
        [SerializeField] private string m_ServerIP = ""; // 10.2.196.117

        /// <summary>
        /// tcp port
        /// </summary>
        [SerializeField] private int m_TcpPort = -1; // 9000

        /// <summary>
        /// kcp port
        /// </summary>
        [SerializeField] private int m_KcpPort = -1;


        public bool EnableRpcTimeout => m_EnableRpcTimeout;
        public int ReconnectNum => m_ReconnectNum;
        public string ServerIP => m_ServerIP;
        public int TcpPort => m_TcpPort;
        public int KcpPort => m_KcpPort;

        private void Update()
        {
            OneThreadSyncContext.Instance.Update();
            TimerManager.Instance.AdvanceClock();
        }
    }
}