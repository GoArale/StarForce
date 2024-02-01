using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class TimerManager
    {
        public static TimerManager Instance { get; } = new();

        private long m_CurTime;
        private readonly TimingWheel m_FirstLevelWheel; // 第一级时间轮
        private readonly int[] m_WheelSize = { 1 << 12, 1 << 10, 1 << 10 }; // 每一级时间轮的时间格个数
        
        private List<TimerTask> m_RunListWriter = new(); // 立即执行的定时器(writer)
        private List<TimerTask> m_RunListReader = new(); // 立即执行的定时器(reader)

        private ITimeSource m_TimeSrc; // 时间源
        
        private TimerManager(ITimeSource src = null, int [] wheelSize = null)
        {
            m_TimeSrc = src ?? new UtcTimeSource();
            m_CurTime = m_TimeSrc.GetTime();
        
            if (wheelSize != null)
            {
                m_WheelSize = wheelSize;
            }
            m_FirstLevelWheel = GenerateWheel(0, 1);
        }
        
        /// <summary>
        /// 以 intervalMs 间隔执行任务
        /// </summary>
        /// <param name="intervalMs"></param>
        /// <param name="d"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TimerTask AddTimer(long intervalMs, Action d, int count = 1)
        {
            if (intervalMs <= 0)
            {
                intervalMs = 1;
            }
        
            var trigger = m_TimeSrc.GetTime() + intervalMs;
            var task = new TimerTask(trigger, intervalMs, count, d);
            AddTask(task);
            return task;
        }
        
        public TimerTask AddTimer(TimeSpan sp, Action d, int count = 1)
        {
            return AddTimer((long)sp.TotalMilliseconds, d, count);
        }
        
        /// <summary>
        /// delayMs 时间后, 以 intervalMs 间隔执行任务
        /// </summary>
        /// <param name="delayMs"></param>
        /// <param name="intervalMs"></param>
        /// <param name="d"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TimerTask AddDelayTimer(long delayMs, long intervalMs, Action d, int count = 1)
        {
            if (intervalMs <= 0)
            {
                intervalMs = 1;
            }
            if (delayMs < 0)
            {
                delayMs = 0;
            }
            var trigger = m_TimeSrc.GetTime() + delayMs;
            var task = new TimerTask(trigger, intervalMs, count, d);
            AddTask(task);
            return task;
        }
        
        public TimerTask AddDelayTimer(TimeSpan delay, TimeSpan interval, Action d, int count = 1)
        {
            return AddDelayTimer((long)delay.TotalMilliseconds, (long)interval.TotalMilliseconds, d, count);
        }
        
        internal void AddTask(TimerTask task)
        {
            if (task.m_Trigger <= m_CurTime)
            {
                // 只要机器物理时间不回退，这里不会执行；
                // 这里主要处理由于时间回退引起的定时器错乱问题
                m_RunListWriter.Add(task);
            }
            else
            {
                m_FirstLevelWheel.AddTask(task.m_Trigger - m_CurTime, task);
            }
        }
        
        internal void AddTaskToWheel(TimerTask task)
        {
            m_FirstLevelWheel.AddTask(task.m_Trigger - m_CurTime, task);
        }
        
        private void CheckRunList()
        {
            if (m_RunListWriter.Count > 0)
            {
                (m_RunListReader, m_RunListWriter) = (m_RunListWriter, m_RunListReader);

                foreach (TimerTask task in m_RunListReader)
                {
                    if (task.m_Cancelled == false)
                    {
                        task.Run(this);
                    }
                }
                m_RunListReader.Clear();
            }
        }
        
        public void AdvanceClock()
        {
            var now = m_TimeSrc.GetTime();
            if (now <= m_CurTime)
            {
                return;
            }
            var advanceMs = now - m_CurTime;
        
            // 时间轮Tick
            for (var i = 0; i < advanceMs; ++i)
            {
                CheckRunList();
                m_CurTime++;
                m_FirstLevelWheel.OnTick();
            }
        }
        
        internal TimingWheel GenerateWheel(int level, long tickMs)
        {
            var wheelSize = level < m_WheelSize.Length ? m_WheelSize[level] : m_WheelSize[^1];
            return new TimingWheel(this, level, tickMs, wheelSize);
        }
        
        public void SetTimeSource(ITimeSource timeSrc)
        {
            m_TimeSrc = timeSrc;
        }
        
        public ITimeSource GetTimeSource()
        {
            return m_TimeSrc;
        }
    }
}