using System;

namespace GameFramework
{
    public class TimingWheel
    {
        private readonly long m_TickMs; // 时间槽跨度单位
        private readonly int m_TickMsBitMask; // 时间槽跨度单位掩码
        private readonly int m_WheelSize; // 时间槽数量
        private readonly int m_WheelSizeBitMask; // 时间槽数量掩码
        private int m_CurCursor; // 表盘指针游标
        private readonly long m_Duration; // 每轮持续时间
        private readonly TimerTask[] m_Slots; // 任务链表槽
        private readonly TimerManager m_TimerManager; // 定时器管理器
        private TimingWheel m_OverflowWheel; // 高一级时间轮
        private readonly int m_Offset; // 计算槽位时的偏移, 第一层为0, 其它层为1
        private readonly int m_Level; // 层级 [0~n]级
        
        public TimingWheel(TimerManager mgr, int level, long tickMs, int wheelSize)
        {
            m_TickMs = tickMs;
            m_WheelSize = wheelSize;
        
            if ((m_TickMs & (m_TickMs - 1)) != 0)
            {
                throw new Exception("tickMs must power of 2");
            }
            m_TickMsBitMask = (int)Math.Log((double)m_TickMs, 2);
        
            if ((m_WheelSize & (m_WheelSize -1)) != 0)
            {
                throw new Exception("wheelSize must power of 2");
            }
            m_WheelSizeBitMask = m_WheelSize - 1;
        
            m_Duration = m_WheelSize * m_TickMs;
            m_Slots = new TimerTask[m_WheelSize];
            m_Offset = (level == 0) ? 0 : 1;
        
            m_TimerManager = mgr;
            m_OverflowWheel = null;
            m_Level = level;
        }
        
        public void AddTask(long advanceMs, TimerTask task)
        {
            if (advanceMs <= m_Duration)
            {
                var index= (int)(((advanceMs - m_Offset) >> m_TickMsBitMask) + m_CurCursor) & m_WheelSizeBitMask;
        
                if (m_Slots[index] == null)
                {
                    m_Slots[index] = task;
                }
                else
                {
                    m_Slots[index].Push(task);
                }
                //Log.Information($"Add Timer {task.m_dubugID} DelayMs: {delay} Level: {m_level} Slot: {index}");
            }
            else
            {
                if (m_OverflowWheel == null)
                {
                    m_OverflowWheel = m_TimerManager.GenerateWheel(m_Level + 1, m_WheelSize * m_TickMs);
                }
                m_OverflowWheel.AddTask(advanceMs, task);
            }
        }
        
        public void OnTick()
        {
            // 先走游标
            m_CurCursor = (m_CurCursor + 1) & m_WheelSizeBitMask;
        
            var task = m_Slots[m_CurCursor];
            m_Slots[m_CurCursor] = null;
            TimerTask nextTask = null;
            if (m_Level == 0)
            {
                // 执行定时任务
                while (task != null)
                {
                    nextTask = task.Next();
                    if (task.Cancelled == false)
                    {
                        task.Run(m_TimerManager);
                    }
                    task = nextTask;
                }
            }
            else
            {
                // 降级定时任务，依次降级到低层级时间轮中
                while (task != null)
                {
                    nextTask = task.Next();
                    if (task.Cancelled == false)
                    {
                        m_TimerManager.AddTaskToWheel(task);
                    }
                    task = nextTask;
                }
            }
        
            // 当前时间轮转完一圈，触发下级轮子OnTick
            if (m_CurCursor == 0 && m_OverflowWheel != null)
            {
                m_OverflowWheel.OnTick();
            }
        }
    }
}