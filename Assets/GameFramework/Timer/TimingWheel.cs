using System;

namespace GameFramework.Timer
{
    public class TimingWheel
    {
        // internal long m_slotTime;                        // 一个槽的时间长度
        // internal int m_slotTimeBit;                     // 一个槽的时间长度对应二进制长度
        // internal int m_slotNum;                         // 槽的个数
        // internal int m_slotNumBitMask;                  // 槽数对应的二进制mask
        // internal long m_wheelTime;                      // 一轮的时间
        // internal int m_currentCursor = 0;               // 当前游标位置
        // internal TimerTask[] m_slots;                   // 链表槽
        // internal int m_offset = 0;                      // 计算槽位时的偏移，第一层为0，其他层为1
        //
        // TimerManager m_timermgr;                            // 定时器管理类
        // internal TimingWheel m_nextWheel;                 // 下一级时间轮
        // internal int m_level;                           // 当前是第几级时间轮
        //
        // public TimingWheel(TimerManager mgr, int level, long slotTime, int slotNum)
        // {
        //     m_slotTime = slotTime;
        //     m_slotNum = slotNum;
        //
        //
        //     if ((m_slotTime & (m_slotTime - 1)) != 0)
        //     {
        //         throw new Exception($"slot must power of 2");
        //     }
        //     m_slotTimeBit = (int)Math.Log((double)m_slotTime, 2);
        //
        //     if ((m_slotNum & (m_slotNum -1)) != 0)
        //     {
        //         throw new Exception($"slot Num must power of 2");
        //     }
        //     m_slotNumBitMask = m_slotNum - 1;
        //
        //     m_wheelTime = m_slotNum * m_slotTime;
        //     m_slots = new TimerTask[m_slotNum];
        //     m_offset = (level == 0) ? 0 : 1;
        //
        //     m_timermgr = mgr;
        //     m_nextWheel = null;
        //     m_level = level;
        // }
        //
        // public void AddTask(long delay, TimerTask task)
        // {
        //     if (delay <= m_wheelTime)
        //     {
        //         int index= (int)(((delay - m_offset) >> m_slotTimeBit) + m_currentCursor) & m_slotNumBitMask;
        //
        //         if (m_slots[index] == null)
        //         {
        //             m_slots[index] = task;
        //         }
        //         else
        //         {
        //             m_slots[index].Push(task);
        //         }
        //         //Log.Information($"Add Timer {task.m_dubugID} DelayMs: {delay} Level: {m_level} Slot: {index}");
        //     }
        //     else
        //     {
        //         if (m_nextWheel == null)
        //         {
        //             m_nextWheel = m_timermgr.GenerateWheel(m_level + 1, m_slotNum * m_slotTime);
        //         }
        //         m_nextWheel.AddTask(delay, task);
        //     }
        // }
        //
        // public void OnTick()
        // {
        //     // 先走游标
        //     m_currentCursor = (m_currentCursor + 1) & m_slotNumBitMask;
        //
        //     // 执行定时器或者时间轮降级
        //     TimerTask task = m_slots[m_currentCursor];
        //     m_slots[m_currentCursor] = null;
        //     TimerTask nextTask = null;
        //     if (m_level == 0)
        //     {
        //         while (task != null)
        //         {
        //             nextTask = task.Next();
        //             if (task.m_canceled == false)
        //             {
        //                 task.Run(m_timermgr);
        //             }
        //             task = nextTask;
        //         }
        //     }
        //     else
        //     {
        //         // 降级,重新放到前几级轮子
        //         while (task != null)
        //         {
        //             nextTask = task.Next();
        //             if (task.m_canceled == false)
        //             {
        //                 m_timermgr.AddTaskToWheel(task);
        //             }
        //             task = nextTask;
        //         }
        //     }
        //
        //     // 当前时间轮转完一圈，触发下级轮子OnTick
        //     if (m_currentCursor == 0 && m_nextWheel != null)
        //     {
        //         m_nextWheel.OnTick();
        //     }
        // }
    }
}