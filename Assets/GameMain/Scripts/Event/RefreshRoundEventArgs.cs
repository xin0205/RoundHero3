using GameFramework;
using GameFramework.Event;


namespace RoundHero
{
    public class RefreshRoundEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshRoundEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshRoundEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshRoundEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}