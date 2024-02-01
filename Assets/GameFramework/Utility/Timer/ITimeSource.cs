namespace GameFramework
{
    public interface ITimeSource
    {
        long GetTime();
    }
    
    public class UtcTimeSource : ITimeSource
    {
        public long GetTime()
        {
            return Utility.UtcTime.GetNowMilliSecond();
        }
    }

    public class GameTimeSource : ITimeSource
    {
        public long GetTime()
        {
            return Utility.GameTime.GetNowMilliSecond();
        }
    }
}