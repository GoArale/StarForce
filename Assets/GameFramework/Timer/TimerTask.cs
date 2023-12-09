using System;

namespace GameFramework.Timer
{
    public class TimerTask
    {
        // public long m_expiration;             // 到期时间
        // public long m_interval;               // 定时间隔
        // internal int m_count;                   // 触发次数
        // internal int m_triggeredCount;
        // internal bool m_canceled;               // 是否被取消
        //
        // Action m_delegate;
        //
        // TimerTask m_preTask;
        // TimerTask m_nextTask;
        //
        // public int m_ID;
        //
        // public static int increntID;
        //
        // public TimerTask(long expiration, long delayMs, int count, Action d)
        // {
        //     m_expiration = expiration;
        //     m_interval = delayMs;
        //     m_count = count;
        //     m_delegate = d;
        //
        //     m_preTask = this;
        //     m_nextTask = null;
        //
        //     m_ID = ++increntID;
        // }
        //
        // internal void ResetList()
        // {
        //     m_preTask = this;
        //     m_nextTask = null;
        // }
        //
        //
        // internal TimerTask Next()
        // {
        //     if (m_nextTask == null)
        //     {
        //         return null;
        //     }
        //     m_nextTask.m_preTask = m_preTask;
        //
        //     var task = m_nextTask;
        //
        //     m_preTask = this;
        //     m_nextTask = null;
        //
        //     return task;
        // }
        //
        // internal void Push(TimerTask task)
        // {
        //     m_preTask.m_nextTask = task;
        //
        //     task.m_nextTask = null;
        //     task.m_preTask = m_preTask;
        //
        //     m_preTask = task;
        // }
        //
        //
        // internal void Run(TimerManager timer)
        // {
        //     try
        //     {
        //         m_delegate();
        //     }
        //     catch (Exception)
        //     {
        //     }
        //     m_triggeredCount++;
        //     if (m_count <= 0 || m_triggeredCount < m_count)
        //     {
        //         m_expiration = timer.GetTimeSource().GetTime() + m_interval;
        //         timer.AddTask(this);
        //     }
        // }
        //
        // public void Cancel()
        // {
        //     m_canceled = true;
        //     m_delegate = null;
        // }
    }
}