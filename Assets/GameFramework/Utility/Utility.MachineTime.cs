using System;

namespace GameFramework
{
    public static partial class Utility
    {
        public static class MachineTime
        {
            /// <summary>
            /// 机器当前时间戳(单位:s)
            /// </summary>
            /// <returns></returns>
            public static long GetNowSecond()
            {
                return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
            }

            /// <summary>
            /// 机器当前时间戳(单位:ms)
            /// </summary>
            /// <returns></returns>
            public static long GetNowMilliSecond()
            {
                return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
            }

            private static readonly DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();

            /// <summary>
            /// 时间转换为时间戳(单位:s)
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static long DateTimeToTimestampSec(DateTime time)
            {
                return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000; 
            }
            
            /// <summary>
            /// 时间戳转换时间(对应设置的时区时间)
            /// </summary>
            /// <param name="ts"></param>
            /// <returns></returns>
            public static DateTime TimestampSecToDateTime(long ts)
            {
                return StartTime.AddSeconds(ts);
            }

            /// <summary>
            /// 时间转换为时间戳(单位:ms)
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static long DateTimeToTimestampMs(DateTime time)
            {
                return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000; 
            }
            
            /// <summary>
            /// 时间戳转换时间(对应设置的时区时间)
            /// </summary>
            /// <param name="ts"></param>
            /// <returns></returns>
            public static DateTime TimestampMsToDateTime(long ts)
            {
                return StartTime.AddMilliseconds(ts);
            }
        }
    }
}