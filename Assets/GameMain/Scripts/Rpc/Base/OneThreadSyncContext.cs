using System;
using System.Collections.Concurrent;
using System.Threading;

namespace GameMain.Rpc
{
    public class OneThreadSyncContext : SynchronizationContext
    {
        public static OneThreadSyncContext Instance { get; } = new();

        private readonly ConcurrentQueue<Action> m_TaskQueue = new();
        private Action m_Action;
        private int m_NumPerFrame = 1000; // 每帧处理任务数量， 防止单帧耗时太长

        public void Update()
        {
            for (var i = 0; i < m_NumPerFrame; i++)
            {
                if (!m_TaskQueue.TryDequeue(out m_Action))
                {
                    return;
                }

                m_Action();
            }
        }

        public void SetNumPerFrame(int num)
        {
            m_NumPerFrame = num;
        }

        public override void Post(SendOrPostCallback cb, object state)
        {
            // 即使任务由主线程添加,也从队列走一遍
            m_TaskQueue.Enqueue(() => { cb(state); });
        }

        public void Post(Action action)
        {
            m_TaskQueue.Enqueue(action);
        }
    }
}