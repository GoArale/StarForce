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
        
        
        
        public bool EnableRpcTimeout => m_EnableRpcTimeout;
        public int ReconnectNum => m_ReconnectNum;

        private void Update()
        {
            OneThreadSyncContext.Instance.Update();
            TimerManager.Instance.AdvanceClock();
        }
    }
}