using System;
using System.Collections.Generic;

namespace GameFramework.Timer
{
    public class TimerManager
    {
        public static TimerManager Instance { get; } = new TimerManager();

        // long m_currTime;
        // TimingWheel m_firstLevelWheel;                                                        // 第一级轮子
        // private int[] m_wheelSlotNum = new int[] { 1 << 12, 1 << 10, 1 << 10};              // 每一级槽数
        //
        //
        // private List<TimerTask> m_runListWriter = new List<TimerTask>();                    // 立即执行的定时器(writer)
        // private List<TimerTask> m_runListReader = new List<TimerTask>();                    // 立即执行的定时器(reader)
        //
        // ITimeSource m_timeSrc;                                                              // 时间获取源
        // public ITimeSource GetTimeSource() { return m_timeSrc; }
        // internal class RealTimeSource : ITimeSource
        // {
        //     public long GetTime()
        //     {
        //         return UtilTime.GetNowMilliSecond();
        //     }
        //     public long Wait(long time)
        //     {
        //         System.Threading.Thread.Sleep((int)time);
        //         return UtilTime.GetNowMilliSecond();
        //     }
        // }
        //
        // public TimerManager(ITimeSource src = null, int [] wheelSlotNum = null)
        // {
        //     m_timeSrc = src ?? new RealTimeSource();
        //     m_currTime = m_timeSrc.GetTime();
        //
        //     if (wheelSlotNum != null)
        //     {
        //         m_wheelSlotNum = wheelSlotNum;
        //     }
        //     m_firstLevelWheel = GenerateWheel(0, 1);
        // }
        //
        // public void SetTimeSource(ITimeSource src)
        // {
        //     m_timeSrc = src;
        // }
        //
        // public TimerTask AddTimer(long interval, Action d, int count = 1)
        // {
        //     if (interval <= 0)
        //     {
        //         interval = 1;
        //     }
        //
        //     var expiretime = m_timeSrc.GetTime() + interval;
        //     var task = new TimerTask(expiretime, interval, count, d);
        //     AddTask(task);
        //     return task;
        // }
        //
        // public TimerTask AddTimer(TimeSpan sp, Action d, int count = 1)
        // {
        //     return AddTimer((long)sp.TotalMilliseconds, d, count);
        // }
        //
        // public TimerTask AddTimer(long delayMs, long interval, Action d, int count = 1)
        // {
        //     if (interval <= 0)
        //     {
        //         interval = 1;
        //     }
        //     if (delayMs <= 0)
        //     {
        //         delayMs = 1;
        //     }
        //     var expiretime = m_timeSrc.GetTime() + delayMs;
        //     var task = new TimerTask(expiretime, interval, count, d);
        //     AddTask(task);
        //     return task;
        // }
        //
        // public TimerTask AddTimer(TimeSpan delay, TimeSpan interval, Action d, int count = 1)
        // {
        //     return AddTimer((long)delay.TotalMilliseconds, (long)interval.TotalMilliseconds, d, count);
        // }
        //
        // internal void AddTask(TimerTask task)
        // {
        //     // 只要机器物理时间不回退，这里走不到(delayms > 0, 当前时间递增)
        //     // 加上这一句， 防止由于时间回退引起定时器错乱
        //     if (task.m_expiration <= m_currTime)
        //     {
        //         m_runListWriter.Add(task);
        //     }
        //     else
        //     {
        //         m_firstLevelWheel.AddTask(task.m_expiration - m_currTime, task);
        //     }
        // }
        //
        // internal void AddTaskToWheel(TimerTask task)
        // {
        //     m_firstLevelWheel.AddTask(task.m_expiration - m_currTime, task);
        // }
        //
        // private void CheckRunList()
        // {
        //     if (m_runListWriter.Count > 0)
        //     {
        //         var temp = m_runListReader;
        //         m_runListReader = m_runListWriter;
        //         m_runListWriter = temp;
        //
        //         foreach (var task in m_runListReader)
        //         {
        //             if (task.m_canceled == false)
        //             {
        //                 task.Run(this);
        //             }
        //         }
        //         m_runListReader.Clear();
        //     }
        // }
        //
        // public void OnTick()
        // {
        //     var now = m_timeSrc.GetTime();
        //     if (now <= m_currTime)
        //     {
        //         return;
        //     }
        //     long deltaTime = now - m_currTime;
        //
        //     // 时间轮Tick
        //     for (long i = 0; i < deltaTime; ++i)
        //     {
        //         CheckRunList();
        //         m_currTime++;
        //         m_firstLevelWheel.OnTick();
        //     }
        // }
        //
        // internal TimingWheel GenerateWheel(int level, long slotTime)
        // {
        //     int slotNum = level < m_wheelSlotNum.Length ? m_wheelSlotNum[level] : m_wheelSlotNum[m_wheelSlotNum.Length - 1];
        //     return new TimingWheel(this, level, slotTime, slotNum);
        // }
    }
}