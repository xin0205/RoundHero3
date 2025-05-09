using GameFramework;
using GameFramework.Event;


namespace RoundHero
{
    public class RefreshTutorialEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshTutorialEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshTutorialEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshTutorialEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}