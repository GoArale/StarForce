using System;

namespace GameFramework
{
    public static partial class Utility
    {
        public static class GameTime
        {
            // GM调时间的偏移值
            private static long TimeOffsetSecond = 0;

            // 跨天重置的偏移时间(比如凌晨5点 为 5 * 3600)
            private static int PassDayHour = 5;
            private static TimeSpan PassDayOffset;
            private static long PassDayOffsetSecond = 0;

            public static void SetPassDayHour(int hour)
            {
                PassDayHour = hour;
                PassDayOffset = TimeSpan.FromHours(hour);
                PassDayOffsetSecond = (long)PassDayOffset.TotalSeconds;
            }

            public static void GMSetServerTime(long setTime)
            {
                var machineTime = UtcTime.GetNowSecond();
                TimeOffsetSecond = setTime - machineTime;
            }

            public static long GetNowSecond()
            {
                return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000 + TimeOffsetSecond;
            }

            public static long GetNowMilliSecond()
            {
                return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000 + TimeOffsetSecond * 1000;
            }

            public static DateTime GetNowDateTime()
            {
                return DateTime.Now.AddSeconds(TimeOffsetSecond);
            }
            
            // 是否在同一天, 时间戳单位秒(以策划配置的时间为跨天)
            public static bool IsInSameDay(long t1, long t2)
            {
                DateTime dt1 = UtcTime.TimestampSecToDateTime(t1) - PassDayOffset;
                DateTime dt2 = UtcTime.TimestampSecToDateTime(t2) - PassDayOffset;
            
                return (dt1.Year == dt2.Year) && (dt1.DayOfYear == dt2.DayOfYear);
            }
            
            public static bool IsInSameHour(long t1, long t2)
            {
                DateTime dt1 = UtcTime.TimestampSecToDateTime(t1);
                DateTime dt2 = UtcTime.TimestampSecToDateTime(t2);
                return (dt1.Year == dt2.Year) && (dt1.DayOfYear == dt2.DayOfYear) && (dt1.Hour == dt2.Hour);
            }
            
            public static bool IsInSameWeek(long t1, long t2)
            {
                DateTime dt1 = UtcTime.TimestampSecToDateTime(t1) - PassDayOffset;
                DateTime dt2 = UtcTime.TimestampSecToDateTime(t2) - PassDayOffset;
            
                var calendar = new System.Globalization.GregorianCalendar();
                var week1 = calendar.GetWeekOfYear(dt1, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var week2 = calendar.GetWeekOfYear(dt2, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                return (dt1.Year == dt2.Year) && (week1 == week2);
            }
            
            public static bool IsInSameMonth(long t1, long t2)
            {
                DateTime dt1 = UtcTime.TimestampSecToDateTime(t1) - PassDayOffset;
                DateTime dt2 = UtcTime.TimestampSecToDateTime(t2) - PassDayOffset;
            
                return (dt1.Year == dt2.Year) && (dt1.Month == dt2.Month);
            }
            
            public static DateTime GetNextHourTime(int minute, int second)
            {
                DateTime now = GetNowDateTime();
                var dt = new DateTime(now.Year, now.Month, now.Day, now.Hour, minute, second);
                return now <= dt ? dt : dt.AddHours(1);
            }
            
            public static DateTime GetNextDayTime(int hour, int minute, int second)
            {
                DateTime now = GetNowDateTime();
                var dt = new DateTime(now.Year, now.Month, now.Day, hour, minute, second);
                return now <= dt ? dt : dt.AddDays(1);
            }
            
            // 获取下次每日重置时间戳
            public static long GetNextResetTime()
            {
                return UtcTime.DateTimeToTimestampSec(GetNextDayTime(PassDayHour, 0, 0));
            }
        }
    }
}