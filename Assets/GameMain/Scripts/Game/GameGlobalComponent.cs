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

        public bool EnableRpcTimeout => m_EnableRpcTimeout;

        private void Update()
        {
            OneThreadSyncContext.Instance.Update();
            TimerManager.Instance.AdvanceClock();
        }
    }
}