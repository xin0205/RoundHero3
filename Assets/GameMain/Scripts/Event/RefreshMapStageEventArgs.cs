using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class RefreshMapStageEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshMapStageEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshMapStageEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshMapStageEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}