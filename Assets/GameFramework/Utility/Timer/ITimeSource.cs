namespace GameFramework
{
    public interface ITimeSource
    {
        long GetTime();
    }
    
    public class MachineTimeSource : ITimeSource
    {
        public long GetTime()
        {
            return Utility.MachineTime.GetNowMilliSecond();
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