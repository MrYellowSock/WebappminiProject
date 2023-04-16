namespace BookStoreApi.Util{
    /// JS compattible
    public class Timing{
        static public long fromTick(long tick)
        {
            return (tick - 621355968000000000) / 10000;
        }
        static public long now(){
            return Timing.fromTick(DateTime.UtcNow.Ticks);
        }
        static public long nowAddMinute(long minute)
        {
            return Timing.now() + minute * 60*1000;
        }
    }
}