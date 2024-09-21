using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class RefreshUnitDataEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshUnitDataEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshUnitDataEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshUnitDataEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}