using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class RefreshCardInfoEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshCardInfoEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshCardInfoEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshCardInfoEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}