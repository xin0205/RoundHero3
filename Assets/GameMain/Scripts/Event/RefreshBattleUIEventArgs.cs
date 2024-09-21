using GameFramework;
using GameFramework.Event;

namespace RoundHero
{
    public class RefreshBattleUIEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshBattleUIEventArgs).GetHashCode();
        public override int Id => EventId;
        
        public static RefreshBattleUIEventArgs Create()
        {
            var args = ReferencePool.Acquire<RefreshBattleUIEventArgs>();
      
            return args;
        }
        
        public override void Clear()
        {

        }
    }
}