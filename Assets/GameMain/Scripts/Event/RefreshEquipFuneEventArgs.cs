using GameFramework;
using GameFramework.Event;


namespace RoundHero
{
    public class RefreshEquipFuneEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshEquipFuneEventArgs).GetHashCode();
        public override int Id => EventId;

        
        public static RefreshEquipFuneEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshEquipFuneEventArgs>();
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}