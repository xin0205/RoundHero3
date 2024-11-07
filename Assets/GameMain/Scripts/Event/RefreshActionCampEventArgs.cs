using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class RefreshActionCampEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshActionCampEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public bool IsUs;
        
        public static RefreshActionCampEventArgs Create(bool isUs)
        {
            var args = ReferencePool.Acquire<RefreshActionCampEventArgs>();
            args.IsUs = isUs;
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}