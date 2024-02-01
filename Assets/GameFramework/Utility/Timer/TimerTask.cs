using System;
using System.Reflection;

namespace GameFramework
{
    public class TimerTask
    {
        internal long m_Trigger; // 任务触发时间
        private readonly long m_IntervalMs; // 定时执行间隔
        private readonly int m_Count; // 计划触发次数
        private int m_Counter; // 完成触发次数

        private Action m_Delegate; // 任务委托内容
        private TimerTask m_PrevTask;
        private TimerTask m_NextTask;

        internal bool m_Cancelled; // 任务是否取消
        private int m_Index; // 命中时间轮索引
        private uint m_Level; // 命中时间轮层级
        
        public TimerTask(long trigger, long intervalMs, int count, Action d)
        {
            m_Trigger = trigger;
            m_IntervalMs = intervalMs;
            m_Count = count;
            m_Delegate = d;

            m_PrevTask = this;
            m_NextTask = null;
        }
        
         internal TimerTask Next()
        {
            if (m_NextTask == null)
            {
                return null;
            }
            m_NextTask.m_PrevTask = m_PrevTask;
            var task = m_NextTask;
        
            m_PrevTask = this;
            m_NextTask = null;
        
            return task;
        }
        
        internal void Push(TimerTask task)
        {
            m_PrevTask.m_NextTask = task;
        
            task.m_NextTask = null;
            task.m_PrevTask = m_PrevTask;
        
            m_PrevTask = task;
        }
        
        public void Cancel()
        {
            m_Cancelled = true;
            m_Delegate = null;
        }
        
        internal void Run(TimerManager timer)
        {
            m_Delegate();
            m_Counter++;
            if (m_Count <= 0 || m_Counter < m_Count)
            {
                // m_TriggerMs += m_IntervalMs
                // 以任务实行完成后的时间点 + 定时时间 更合理一些
                m_Trigger = timer.GetTimeSource().GetTime() + m_IntervalMs;
                timer.AddTask(this);
            }
        }
    }
}